#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>

// ----------------------------
// 전역 변수
// ----------------------------
static AVAssetWriter *writer = nil;
static AVAssetWriterInput *videoInput = nil;
static AVAssetWriterInputPixelBufferAdaptor *adaptor = nil;
static CADisplayLink *displayLink = nil;

static CFAbsoluteTime startTime;
static CGSize screenSize;

static UIView *recIndicator = nil;
static UIButton *stopButton = nil;

// pixel buffer attrs 전역 저장
static NSDictionary *pixelAttrs = nil;

// ----------------------------
// 함수 선언 (컴파일 에러 방지)
// ----------------------------
extern "C" {
    void StartCGRecord();
    void StopCGRecord();
}


// ----------------------------
// 녹화 종료
// ----------------------------
void StopCGRecord() {

    NSLog(@"[CGRecorder] STOP");

    // UI 제거
    dispatch_async(dispatch_get_main_queue(), ^{
        if (recIndicator) { [recIndicator removeFromSuperview]; recIndicator = nil; }
        if (stopButton)   { [stopButton removeFromSuperview];   stopButton = nil; }
    });

    // 화면 캡처 중지
    if (displayLink) {
        [displayLink invalidate];
        displayLink = nil;
    }

    // Writer 종료
    if (videoInput) [videoInput markAsFinished];
    if (writer) {
        [writer finishWritingWithCompletionHandler:^{
            NSLog(@"[CGRecorder] FINISHED WRITING");
        }];
    }
}


// ----------------------------
// 녹화 시작
// ----------------------------
void StartCGRecord() {

    NSLog(@"[CGRecorder] START");

    screenSize = [UIScreen mainScreen].bounds.size;

    // 출력 파일 경로
    NSString *outputPath =
      [NSTemporaryDirectory() stringByAppendingPathComponent:@"cg_record.mp4"];
    NSURL *outputURL = [NSURL fileURLWithPath:outputPath];
    [[NSFileManager defaultManager] removeItemAtURL:outputURL error:nil];

    // Writer 설정
    writer = [AVAssetWriter assetWriterWithURL:outputURL
                                      fileType:AVFileTypeMPEG4
                                         error:nil];

    NSDictionary *videoSettings = @{
        AVVideoCodecKey: AVVideoCodecTypeH264,
        AVVideoWidthKey: @(screenSize.width * 2),
        AVVideoHeightKey: @(screenSize.height * 2),
        AVVideoCompressionPropertiesKey: @{
            AVVideoAverageBitRateKey: @(8 * 1024 * 1024)
        }
    };

    videoInput =
    [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo
                                       outputSettings:videoSettings];
    videoInput.expectsMediaDataInRealTime = YES;

    pixelAttrs = @{
        (NSString*)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA),
        (NSString*)kCVPixelBufferWidthKey: @(screenSize.width * 2),
        (NSString*)kCVPixelBufferHeightKey: @(screenSize.height * 2)
    };

    adaptor = [AVAssetWriterInputPixelBufferAdaptor
               assetWriterInputPixelBufferAdaptorWithAssetWriterInput:videoInput
               sourcePixelBufferAttributes:pixelAttrs];

    [writer addInput:videoInput];
    [writer startWriting];
    [writer startSessionAtSourceTime:kCMTimeZero];

    startTime = CFAbsoluteTimeGetCurrent();

    // ----------------------------
    // 네이티브 UI (REC 표시 + 투명 Stop 버튼)
    // ----------------------------
    dispatch_async(dispatch_get_main_queue(), ^{

        UIWindow *window = UIApplication.sharedApplication.keyWindow;
        if (!window) return;

        // 빨간 점
        recIndicator = [[UIView alloc] initWithFrame:CGRectMake(20, 50, 20, 20)];
        recIndicator.backgroundColor = [UIColor redColor];
        recIndicator.layer.cornerRadius = 10;
        [window addSubview:recIndicator];

        // 종료 버튼
        stopButton = [[UIButton alloc] initWithFrame:CGRectMake(20, 50, 120, 120)];
        stopButton.backgroundColor = [UIColor colorWithWhite:0 alpha:0.01];
        [stopButton addTarget:[NSBlockOperation blockOperationWithBlock:^{
            StopCGRecord();
        }] action:@selector(main) forControlEvents:UIControlEventTouchUpInside];

        [window addSubview:stopButton];
    });

    // ----------------------------
    // DisplayLink 화면 캡처
    // ----------------------------
    displayLink =
    [CADisplayLink displayLinkWithTarget:[NSBlockOperation blockOperationWithBlock:^{

        if (!videoInput || !videoInput.readyForMoreMediaData) return;

        // 화면을 이미지로 렌더링
        UIGraphicsBeginImageContextWithOptions(screenSize, NO, 2.0);
        [[[UIApplication sharedApplication] keyWindow].layer
            renderInContext:UIGraphicsGetCurrentContext()];
        UIImage *frameImg = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();

        // PixelBuffer 생성
        CVPixelBufferRef buffer = NULL;
        CVPixelBufferCreate(kCFAllocatorDefault,
                            screenSize.width * 2,
                            screenSize.height * 2,
                            kCVPixelFormatType_32BGRA,
                            (__bridge CFDictionaryRef)(pixelAttrs),
                            &buffer);

        if (!buffer) return;

        CVPixelBufferLockBaseAddress(buffer, 0);
        void *pixelData = CVPixelBufferGetBaseAddress(buffer);

        CGColorSpaceRef cs = CGColorSpaceCreateDeviceRGB();
        CGContextRef ctx =
        CGBitmapContextCreate(pixelData,
                              screenSize.width * 2,
                              screenSize.height * 2,
                              8,
                              CVPixelBufferGetBytesPerRow(buffer),
                              cs,
                              kCGBitmapByteOrder32Little |
                              kCGImageAlphaPremultipliedFirst);

        CGContextDrawImage(ctx,
                           CGRectMake(0, 0,
                                      screenSize.width * 2,
                                      screenSize.height * 2),
                           frameImg.CGImage);

        CGColorSpaceRelease(cs);
        CGContextRelease(ctx);
        CVPixelBufferUnlockBaseAddress(buffer, 0);

        // 타임스탬프 계산
        CFTimeInterval now = CFAbsoluteTimeGetCurrent();
        CMTime frameTime = CMTimeMakeWithSeconds(now - startTime, 600);

        [adaptor appendPixelBuffer:buffer withPresentationTime:frameTime];
        CVPixelBufferRelease(buffer);

    }] selector:@selector(main)];

    [displayLink addToRunLoop:[NSRunLoop mainRunLoop]
                      forMode:NSDefaultRunLoopMode];
}
