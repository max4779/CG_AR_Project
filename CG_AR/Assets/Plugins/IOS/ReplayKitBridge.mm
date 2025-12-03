#import <ReplayKit/ReplayKit.h>
#import <UIKit/UIKit.h>

static BOOL isRecording = NO;

extern "C" {

void StartRecording() {
    if (@available(iOS 11.0, *)) {

        if (isRecording) return;

        RPScreenRecorder *recorder = [RPScreenRecorder sharedRecorder];
        recorder.microphoneEnabled = YES;

        [recorder startRecordingWithHandler:^(NSError * _Nullable error) {
            if (error) {
                NSLog(@"[ReplayKit] StartRecording ERROR: %@", error.localizedDescription);
            } else {
                NSLog(@"[ReplayKit] Recording Started");
                isRecording = YES;
            }
        }];
    }
}

void StopRecording() {
    if (@available(iOS 11.0, *)) {

        if (!isRecording) return;

        RPScreenRecorder *recorder = [RPScreenRecorder sharedRecorder];

        [recorder stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {

            if (error) {
                NSLog(@"[ReplayKit] StopRecording ERROR: %@", error.localizedDescription);
                isRecording = NO;
                return;
            }

            NSLog(@"[ReplayKit] Recording Stopped");

            // 자동으로 팝업을 띄우지 않기 위해 previewViewController를 무시함
            // 또는 필요하다면 팝업을 띄우도록 아래 코드 사용 가능:
            /*
            UIViewController *root = UIApplication.sharedApplication.keyWindow.rootViewController;
            [root presentViewController:previewViewController animated:YES completion:nil];
             */

            isRecording = NO;
        }];
    }
}

void ShowRECIndicator() {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (@available(iOS 13.0, *)) {

            // 빨간 점 UI 표시
            UIView *indicator = [[UIView alloc] initWithFrame:CGRectMake(20, 40, 18, 18)];
            indicator.backgroundColor = [UIColor redColor];
            indicator.layer.cornerRadius = 9;
            indicator.tag = 987654;

            UIWindow *window = UIApplication.sharedApplication.keyWindow;
            [window addSubview:indicator];
        }
    });
}

void HideRECIndicator() {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIWindow *window = UIApplication.sharedApplication.keyWindow;
        UIView *indicator = [window viewWithTag:987654];
        if (indicator) [indicator removeFromSuperview];
    });
}

}
