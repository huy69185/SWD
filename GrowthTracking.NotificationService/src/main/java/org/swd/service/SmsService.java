package org.swd.service;

import org.swd.dto.request.SmsRequest;
import org.swd.dto.response.NotificationResponse;

public interface SmsService {
    NotificationResponse sendSms(SmsRequest request);
}