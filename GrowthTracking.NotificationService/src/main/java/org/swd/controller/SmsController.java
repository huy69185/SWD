package org.swd.controller;

import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.swd.dto.request.SmsRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.service.SmsService;

@RestController
@RequestMapping("/api/v1/sms")
@RequiredArgsConstructor
public class SmsController {

    private final SmsService smsService;

    @PostMapping
    public ResponseEntity<NotificationResponse> sendSms(@Valid @RequestBody SmsRequest request) {
        NotificationResponse response = smsService.sendSms(request);
        return ResponseEntity.ok(response);
    }
}