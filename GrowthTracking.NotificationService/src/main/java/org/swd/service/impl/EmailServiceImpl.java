// src/main/java/org/swd/service/impl/EmailServiceImpl.java
package org.swd.service.impl;

import jakarta.mail.MessagingException;
import jakarta.mail.internet.MimeMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;
import org.swd.dto.request.EmailRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.dto.response.StatusResponse;
import org.swd.service.EmailService;

import java.time.Instant;
import java.util.Base64;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

@Service
@RequiredArgsConstructor
@Slf4j
public class EmailServiceImpl implements EmailService {
    private final JavaMailSender mailSender;

    // In-memory storage (replace with database in production)
    private final Map<String, StatusResponse> emailStatusMap = new ConcurrentHashMap<>();

    @Override
    public NotificationResponse sendEmail(EmailRequest request) {
        String emailId = UUID.randomUUID().toString();

        // If email is scheduled for future, implement scheduling logic
        if (request.getScheduledTime() != null && request.getScheduledTime().isAfter(Instant.now())) {
            // Schedule for future delivery
            log.info("Email scheduled for future delivery: {}", request.getScheduledTime());
            // Add scheduling implementation here
        } else {
            // Send immediately
            sendEmailAsync(emailId, request);
        }

        // Create initial status
        StatusResponse status = StatusResponse.builder()
                .id(emailId)
                .status("QUEUED")
                .sentAt(null)
                .deliveredAt(null)
                .retryCount(0)
                .build();

        emailStatusMap.put(emailId, status);

        return NotificationResponse.builder()
                .id(emailId)
                .status("QUEUED")
                .timestamp(Instant.now())
                .build();
    }

    @Override
    public StatusResponse getStatus(String id) {
        StatusResponse status = emailStatusMap.get(id);
        if (status == null) {
            log.warn("Status not found for email id: {}", id);
            throw new RuntimeException("Email notification not found with id: " + id);
        }
        return status;
    }

    @Async
    protected void sendEmailAsync(String emailId, EmailRequest request) {
        try {
            MimeMessage message = mailSender.createMimeMessage();
            MimeMessageHelper helper = new MimeMessageHelper(message, true, "UTF-8");

            // Set recipients
            helper.setTo(request.getTo().toArray(new String[0]));
            if (request.getCc() != null && !request.getCc().isEmpty()) {
                helper.setCc(request.getCc().toArray(new String[0]));
            }
            if (request.getBcc() != null && !request.getBcc().isEmpty()) {
                helper.setBcc(request.getBcc().toArray(new String[0]));
            }

            // Set subject and body
            helper.setSubject(request.getSubject());
            helper.setText(request.getBody(), request.isHtml());

            // Add attachments if any
            if (request.getAttachments() != null) {
                for (EmailRequest.Attachment attachment : request.getAttachments()) {
                    byte[] attachmentBytes = Base64.getDecoder().decode(attachment.getContent());
                    helper.addAttachment(attachment.getFileName(),
                            new org.springframework.core.io.ByteArrayResource(attachmentBytes),
                            attachment.getContentType());
                }
            }

            // Update status to SENDING
            updateStatus(emailId, "SENDING", Instant.now(), null);

            // Send email
            mailSender.send(message);

            // Update status to SENT
            updateStatus(emailId, "SENT", Instant.now(), Instant.now());
            log.info("Email sent successfully with id: {}", emailId);

        } catch (MessagingException e) {
            log.error("Failed to send email: {}", e.getMessage(), e);
            updateStatus(emailId, "FAILED", Instant.now(), null);
            // Implement retry logic here if needed
        }
    }

    private void updateStatus(String emailId, String status, Instant sentAt, Instant deliveredAt) {
        StatusResponse currentStatus = emailStatusMap.get(emailId);
        if (currentStatus != null) {
            currentStatus.setStatus(status);
            if (sentAt != null) {
                currentStatus.setSentAt(sentAt);
            }
            if (deliveredAt != null) {
                currentStatus.setDeliveredAt(deliveredAt);
            }
            emailStatusMap.put(emailId, currentStatus);
        }
    }
}