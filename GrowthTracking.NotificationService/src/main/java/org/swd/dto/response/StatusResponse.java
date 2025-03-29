package org.swd.dto.response;


import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class StatusResponse {
    private String id;
    private String status;
    private Instant sentAt;
    private Instant deliveredAt;
    private Integer retryCount;
}