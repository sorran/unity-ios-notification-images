#import <UserNotifications/UserNotifications.h>

void _Plugin_ScheduleLocalNotification(char* instanceId, char* title, char* body, char* attachmentFile, int timeIntervalSecs)
{
    UNMutableNotificationContent* content = [[UNMutableNotificationContent alloc] init];

    content.title = [NSString stringWithUTF8String:title];
    content.body = [NSString stringWithUTF8String:body];
        
    NSString *attachmentRes =  [NSString stringWithUTF8String:attachmentFile];
    if (attachmentRes != nil){
        NSFileManager* fileMgr = [NSFileManager defaultManager];
        BOOL fileExists = [fileMgr fileExistsAtPath:attachmentRes];
        if (fileExists == NO){
            NSLog(@"Missing Attachment %@", attachmentRes);
        }else{
            NSLog(@"Attachment %@", attachmentRes);
            NSURL *URL = [NSURL fileURLWithPath:attachmentRes];
            NSError *attachmentError = nil;
            UNNotificationAttachment* attachment = [UNNotificationAttachment attachmentWithIdentifier: [attachmentRes lastPathComponent]
                                                    URL:URL
                                                    options:nil
                                                     error:&attachmentError];
            if (attachmentError)
            {
                NSLog(@"%@", attachmentError.localizedDescription);
                NSLog(@"Attachment BAD %@", attachmentRes);
            }else{
                content.attachments = @[attachment];
                NSLog(@"Attachment OK %@", attachmentRes);
            }
        }        
    }
    
    UNNotificationTrigger* trigger = [UNTimeIntervalNotificationTrigger triggerWithTimeInterval: timeIntervalSecs repeats:NO];
    
    UNNotificationRequest* request = [UNNotificationRequest requestWithIdentifier: @(instanceId) content: content trigger: trigger];

    UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest: request withCompletionHandler:^(NSError * _Nullable error) {
        if (error != NULL){
            NSLog(@"%@", [error localizedDescription]);
        }            
    }];
}
