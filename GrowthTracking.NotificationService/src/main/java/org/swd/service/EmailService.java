package org.swd.service;


import org.swd.dto.request.EmailRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.dto.response.StatusResponse;

public interface EmailService {
    NotificationResponse sendEmail(EmailRequest request);
    StatusResponse getStatus(String id);
}