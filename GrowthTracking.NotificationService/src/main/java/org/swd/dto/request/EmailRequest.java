package org.swd.dto.request;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class EmailRequest {
    @NotEmpty(message = "Recipients list cannot be empty")
    private List<String> to;
    private List<String> cc;
    private List<String> bcc;

    @NotBlank(message = "Subject is required")
    private String subject;

    @NotBlank(message = "Email body is required")
    private String body;

    private boolean isHtml;
    private List<Attachment> attachments;
    private String priority;
    private Instant scheduledTime;

    @Data
    public static class Attachment {
        private String fileName;
        private String content; // Base64 encoded
        private String contentType;
    }
}
