package org.swd.controller;

import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.swd.dto.request.EmailRequest;
import org.swd.dto.response.NotificationResponse;
import org.swd.dto.response.StatusResponse;
import org.swd.service.EmailService;

@RestController
@RequestMapping("/api/v1/emails")
@RequiredArgsConstructor
public class EmailController {
    private final EmailService emailService;

    @PostMapping
    public ResponseEntity<NotificationResponse> sendEmail(
            @Valid @RequestBody EmailRequest request) {
        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(emailService.sendEmail(request));
    }

    @GetMapping("/{id}")
    public ResponseEntity<StatusResponse> getEmailStatus(@PathVariable String id) {
        return ResponseEntity.ok(emailService.getStatus(id));
    }
}