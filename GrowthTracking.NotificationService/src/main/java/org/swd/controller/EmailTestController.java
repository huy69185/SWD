package org.swd.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import org.swd.dto.request.EmailRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.service.EmailService;

import java.time.Instant;
import java.util.Collections;

@RestController
public class EmailTestController {

    private final EmailService emailService;

    public EmailTestController(EmailService emailService) {
        this.emailService = emailService;
    }

    @GetMapping("/api/v1/test/email")
    public ResponseEntity<NotificationResponse> testEmailService() {
        EmailRequest request = new EmailRequest();
        // Set a sample recipient list
        request.setTo(Collections.singletonList("test@example.com"));
        request.setSubject("Test Email");
        request.setBody("This is a test email sent by the Email Service.");
        request.setHtml(false);
        // Set scheduledTime to current instant to send immediately
        request.setScheduledTime(Instant.now());

        // Call emailService to send email and receive the notification response
        NotificationResponse response = emailService.sendEmail(request);

        return ResponseEntity.ok(response);
    }
}