package org.swd.dto.request;

import jakarta.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class SmsRequest {
    @NotBlank(message = "Phone number is required")
    private String to;

    @NotBlank(message = "Message body is required")
    private String body;

    private Instant scheduledTime;
    private String priority;
}