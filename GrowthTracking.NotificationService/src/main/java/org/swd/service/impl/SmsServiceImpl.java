package org.swd.service.impl;

import com.twilio.Twilio;
import com.twilio.rest.api.v2010.account.Message;
import com.twilio.type.PhoneNumber;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.swd.dto.request.SmsRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.service.SmsService;

import jakarta.annotation.PostConstruct;
import java.time.Instant;
import java.util.UUID;

@Service
@Slf4j
public class SmsServiceImpl implements SmsService {

    @Value("${TWILIO_ACCOUNT_SID}")
    private String accountSid;

    @Value("${TWILIO_AUTH_TOKEN}")
    private String authToken;

    @Value("${TWILIO_PHONE_NUMBER}")
    private String fromPhoneNumber;

    @PostConstruct
    private void init() {
        Twilio.init(accountSid, authToken);
        log.info("Twilio initialized with account {}", accountSid);
    }

    @Override
    public NotificationResponse sendSms(SmsRequest request) {
        try {
            Message message = Message.creator(
                    new PhoneNumber(request.getTo()),
                    new PhoneNumber(fromPhoneNumber),
                    request.getBody()
            ).create();

            log.info("SMS sent successfully with SID: {}", message.getSid());

            return NotificationResponse.builder()
                    .id(message.getSid())
                    .status("SENT")
                    .timestamp(Instant.now())
                    .build();
        } catch (Exception e) {
            log.error("Failed to send SMS: {}", e.getMessage(), e);
            return NotificationResponse.builder()
                    .id(UUID.randomUUID().toString())
                    .status("FAILED")
                    .timestamp(Instant.now())
                    .build();
        }
    }
}