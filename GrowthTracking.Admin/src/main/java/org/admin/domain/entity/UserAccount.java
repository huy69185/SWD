package org.admin.domain.entity;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;
import java.util.UUID;

@Data
@Entity
@Table(name = "UserAccount")
@NoArgsConstructor
@AllArgsConstructor
public class UserAccount {
    @Id
    @Column(name = "UserAccountID")
    private UUID userAccountId;

    @Column(name = "FullName", nullable = false)
    private String fullName;

    @Column(name = "Email", nullable = false, unique = true)
    private String email;

    @Column(name = "PasswordHash")
    private String passwordHash;

    @Column(name = "PhoneNumber")
    private String phoneNumber;

    @Column(name = "Role", nullable = false)
    private String role;

    @Column(name = "CreatedAt")
    private LocalDateTime createdAt;

    @Column(name = "UpdatedAt")
    private LocalDateTime updatedAt;

    @Column(name = "IsActive")
    private Boolean isActive;

    @Column(name = "ProfilePictureUrl")
    private String profilePictureUrl;

    @Column(name = "Address", columnDefinition = "text")
    private String address;

    @Column(name = "Bio", columnDefinition = "text")
    private String bio;

    @Column(name = "EmailVerified")
    private Boolean emailVerified;

    @Column(name = "PhoneVerified")
    private Boolean phoneVerified;

    @Column(name = "OAuth2GoogleId")
    private String oAuth2GoogleId;

    @Column(name = "OAuth2FacebookId")
    private String oAuth2FacebookId;

    @Column(name = "LastLoginAt")
    private LocalDateTime lastLoginAt;

    @PrePersist
    protected void onCreate() {
        if (userAccountId == null) {
            userAccountId = UUID.randomUUID();
        }
        createdAt = LocalDateTime.now();
        updatedAt = LocalDateTime.now();
        if (isActive == null) {
            isActive = true;
        }
        if (emailVerified == null) {
            emailVerified = false;
        }
        if (phoneVerified == null) {
            phoneVerified = false;
        }
    }

    @PreUpdate
    protected void onUpdate() {
        updatedAt = LocalDateTime.now();
    }
}