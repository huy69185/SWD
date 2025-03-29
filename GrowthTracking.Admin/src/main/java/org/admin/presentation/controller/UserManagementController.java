package org.admin.presentation.controller;

import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.admin.application.dto.CreateUserDTO;
import org.admin.application.dto.UserAccountDTO;
import org.admin.application.service.UserManagementService;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequestMapping("/api/admin/users")
@RequiredArgsConstructor
public class UserManagementController {

    private final UserManagementService userManagementService;

    @GetMapping
    public ResponseEntity<Page<UserAccountDTO>> getAllUsers(Pageable pageable) {
        return ResponseEntity.ok(userManagementService.getAllUsers(pageable));
    }

    @GetMapping("/{id}")
    public ResponseEntity<UserAccountDTO> getUserById(@PathVariable UUID id) {
        return ResponseEntity.ok(userManagementService.getUserById(id));
    }

    @PostMapping
    public ResponseEntity<UserAccountDTO> createUser(@Valid @RequestBody CreateUserDTO createUserDTO) {
        UserAccountDTO createdUser = userManagementService.createUser(createUserDTO);
        return new ResponseEntity<>(createdUser, HttpStatus.CREATED);
    }

    @PutMapping("/{id}")
    public ResponseEntity<UserAccountDTO> updateUser(@PathVariable UUID id,
                                                     @Valid @RequestBody CreateUserDTO updateUserDTO) {
        return ResponseEntity.ok(userManagementService.updateUser(id, updateUserDTO));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteUser(@PathVariable UUID id) {
        userManagementService.deleteUser(id);
        return ResponseEntity.noContent().build();
    }

    @PatchMapping("/{id}/toggle-active")
    public ResponseEntity<UserAccountDTO> toggleUserActiveStatus(@PathVariable UUID id) {
        return ResponseEntity.ok(userManagementService.toggleUserActiveStatus(id));
    }
}