#import <UIKit/UIKit.h>
#import <ReplayKit/ReplayKit.h>

UIView *recIndicator;

extern "C" {

    void ShowRECIndicator() {
        dispatch_async(dispatch_get_main_queue(), ^{

            if (recIndicator != nil) return;

            UIWindow *window = UIApplication.sharedApplication.keyWindow;
            CGFloat size = 50;

            recIndicator = [[UIView alloc] initWithFrame:CGRectMake(
                window.bounds.size.width - size - 20,
                40,
                size,
                size
            )];

            recIndicator.backgroundColor = [UIColor colorWithRed:1 green:0 blue:0 alpha:0.9];
            recIndicator.layer.cornerRadius = size / 2;

            UILabel *label = [[UILabel alloc] initWithFrame:recIndicator.bounds];
            label.text = @"REC";
            label.textAlignment = NSTextAlignmentCenter;
            label.textColor = [UIColor whiteColor];
            label.font = [UIFont boldSystemFontOfSize(16)];
            [recIndicator addSubview:label];

            [window addSubview:recIndicator];
        });
    }

    void HideRECIndicator() {
        dispatch_async(dispatch_get_main_queue(), ^{
            [recIndicator removeFromSuperview];
            recIndicator = nil;
        });
    }

    void StartRecording() {
        if (@available(iOS 11.0, *)) {
            RPScreenRecorder *recorder = [RPScreenRecorder sharedRecorder];
            recorder.microphoneEnabled = false;

            [recorder startRecordingWithHandler:^(NSError * _Nullable error) {}];
        }
    }

    void StopRecording() {
        if (@available(iOS 11.0, *)) {
            RPScreenRecorder *recorder = [RPScreenRecorder sharedRecorder];

            [recorder stopRecordingWithHandler:^(RPPreviewViewController *preview, NSError * _Nullable error) {

                UIViewController *rootVC = UIApplication.sharedApplication.keyWindow.rootViewController;
                [rootVC presentViewController:preview animated:YES completion:nil];
            }];
        }
    }
}
