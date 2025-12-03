#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>

static AVAssetWriter *writer;
static AVAssetWriterInput *videoInput;
static AVAssetWriterInputPixelBufferAdaptor *adaptor;
static CADisplayLink *displayLink;

static CFAbsoluteTime startTime;
static CGSize screenSize;

// 네이티브 UI 요소
static UIView *recIndicator;
static UIButton *stopButton;

extern "C" {

void StartRecordingAV() {

    // 1) 화면 크기 가져오기
    screenSize = [UIScreen mainScreen].bounds.size;

    // 2) 출력 mp4 파일 경로
    NSString *outputPath = [NSTemporaryDirectory() stringByAppendingPathComponent:@"unity_record.mp4"];
    NSURL *outputURL = [NSURL fileURLWithPath:outputPath];
    [[NSFileManager defaultManager] removeItemAtURL:outputURL error:nil];

    // 3) AVAssetWriter 설정
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

    videoInput = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo
                                                   outputSettings:videoSettings];
    videoInput.expectsMediaDataInRealTime = YES;

    NSDictionary *pixelAttrs = @{
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

    // 4) 네이티브 오버레이 UI 생성 (영상에는 안 찍힘)
    dispatch_async(dispatch_get_main_queue(), ^{

        UIWindow *window = UIApplication.sharedApplication.keyWindow;

        // 빨간 REC 점
        recIndicator = [[UIView alloc] initWithFrame:CGRectMake(20, 50, 20, 20)];
        recIndicator.backgroundColor = [UIColor redColor];
        recIndicator.layer.cornerRadius = 10;
        [window addSubview:recIndicator];

        // 투명 종료 버튼 (Unity 버튼과 완전 무관)
        stopButton = [[UIButton alloc] initWithFrame:CGRectMake(20, 50, 80, 80)];
        stopButton.backgroundColor = [UIColor colorWithWhite:0 alpha:0.0]; // 완전 투명
        [stopButton addTarget:[NSBlockOperation blockOperationWithBlock:^{
            StopRecordingAV();
        }] action:@selector(main) forControlEvents:UIControlEventTouchUpInside];

        [window addSubview:stopButton];
    });

    // 5) 화면 캡처 (Unity 화면을 mp4로 기록)
    displayLink = [CADisplayLink displayLinkWithTarget:[NSBlockOperation blockOperationWithBlock:^{
        if (!videoInput.readyForMoreMediaData) return;

        UIGraphicsBeginImageContextWithOptions(screenSize, NO, 2.0);
        [[UIApplication sharedApplication].keyWindow.layer
            renderInContext:UIGraphicsGetCurrentContext()];
        UIImage *frameImg = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();

        CVPixelBufferRef buffer = NULL;
        CVPixelBufferCreate(kCFAllocatorDefault,
                            screenSize.width * 2,
                            screenSize.height * 2,
                            kCVPixelFormatType_32BGRA,
                            (__bridge CFDictionaryRef)pixelAttrs,
                            &buffer);

        CVPixelBufferLockBaseAddress(buffer, 0);
        void *pixelData = CVPixelBufferGetBaseAddress(buffer);

        CGColorSpaceRef cs = CGColorSpaceCreateDeviceRGB();
        CGContextRef ctx = CGBitmapContextCreate(pixelData,
                                                 screenSize.width * 2,
                                                 screenSize.height * 2,
                                                 8,
                                                 CVPixelBufferGetBytesPerRow(buffer),
                                                 cs,
                                                 kCGBitmapByteOrder32Little | kCGImageAlphaPremultipliedFirst);

        CGContextDrawImage(ctx,
                           CGRectMake(0,0, screenSize.width*2, screenSize.height*2),
                           frameImg.CGImage);

        CGColorSpaceRelease(cs);
        CGContextRelease(ctx);
        CVPixelBufferUnlockBaseAddress(buffer, 0);

        CFTimeInterval now = CFAbsoluteTimeGetCurrent();
        CMTime frameTime = CMTimeMakeWithSeconds(now - startTime, 600);

        [adaptor appendPixelBuffer:buffer withPresentationTime:frameTime];
        CVPixelBufferRelease(buffer);

    }] selector:@selector(main)];

    [displayLink addToRunLoop:[NSRunLoop mainRunLoop]
                      forMode:NSDefaultRunLoopMode];
}

void StopRecordingAV() {

    // 1) 네이티브 UI 제거 (녹화에는 영향 없음)
    dispatch_async(dispatch_get_main_queue(), ^{
        [recIndicator removeFromSuperview];
        [stopButton removeFromSuperview];
        recIndicator = nil;
        stopButton = nil;
    });

    // 2) 화면 캡처 종료
    [displayLink invalidate];
    displayLink = nil;

    // 3) 녹화 완료
    [videoInput markAsFinished];
    [writer finishWritingWithCompletionHandler:^{
        NSLog(@"[AVRecorder] Recording finished");
    }];
}

}
