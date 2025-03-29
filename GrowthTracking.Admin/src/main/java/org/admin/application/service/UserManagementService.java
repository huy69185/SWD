package org.admin.application.service;

import lombok.RequiredArgsConstructor;
import org.admin.application.dto.CreateUserDTO;
import org.admin.application.dto.UserAccountDTO;
import org.admin.domain.entity.UserAccount;
import org.admin.infrastructure.repository.UserAccountRepository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.UUID;

@Service
@RequiredArgsConstructor
public class UserManagementService {

    private final UserAccountRepository userRepository;
    private final PasswordEncoder passwordEncoder;

    public Page<UserAccountDTO> getAllUsers(Pageable pageable) {
        return userRepository.findAll(pageable)
                .map(this::mapToDTO);
    }

    public UserAccountDTO getUserById(UUID id) {
        return userRepository.findById(id)
                .map(this::mapToDTO)
                .orElseThrow(() -> new RuntimeException("User not found with ID: " + id));
    }

    @Transactional
    public UserAccountDTO createUser(CreateUserDTO createUserDTO) {
        if (userRepository.existsByEmail(createUserDTO.getEmail())) {
            throw new RuntimeException("Email already in use");
        }

        UserAccount newUser = new UserAccount();
        newUser.setFullName(createUserDTO.getFullName());
        newUser.setEmail(createUserDTO.getEmail());
        newUser.setPasswordHash(passwordEncoder.encode(createUserDTO.getPassword()));
        newUser.setPhoneNumber(createUserDTO.getPhoneNumber());
        newUser.setRole(createUserDTO.getRole());
        newUser.setAddress(createUserDTO.getAddress());
        newUser.setBio(createUserDTO.getBio());
        newUser.setProfilePictureUrl(createUserDTO.getProfilePictureUrl());

        return mapToDTO(userRepository.save(newUser));
    }

    @Transactional
    public UserAccountDTO updateUser(UUID id, CreateUserDTO updateUserDTO) {
        UserAccount existingUser = userRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("User not found with ID: " + id));

        // Check if email is being changed and if new email is available
        if (!existingUser.getEmail().equals(updateUserDTO.getEmail()) &&
                userRepository.existsByEmail(updateUserDTO.getEmail())) {
            throw new RuntimeException("Email already in use");
        }

        existingUser.setFullName(updateUserDTO.getFullName());
        existingUser.setEmail(updateUserDTO.getEmail());

        if (updateUserDTO.getPassword() != null && !updateUserDTO.getPassword().isBlank()) {
            existingUser.setPasswordHash(passwordEncoder.encode(updateUserDTO.getPassword()));
        }

        existingUser.setPhoneNumber(updateUserDTO.getPhoneNumber());
        existingUser.setRole(updateUserDTO.getRole());
        existingUser.setAddress(updateUserDTO.getAddress());
        existingUser.setBio(updateUserDTO.getBio());
        existingUser.setProfilePictureUrl(updateUserDTO.getProfilePictureUrl());

        return mapToDTO(userRepository.save(existingUser));
    }

    @Transactional
    public void deleteUser(UUID id) {
        if (!userRepository.existsById(id)) {
            throw new RuntimeException("User not found with ID: " + id);
        }
        userRepository.deleteById(id);
    }

    @Transactional
    public UserAccountDTO toggleUserActiveStatus(UUID id) {
        UserAccount user = userRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("User not found with ID: " + id));

        user.setIsActive(!user.getIsActive());
        return mapToDTO(userRepository.save(user));
    }

    private UserAccountDTO mapToDTO(UserAccount user) {
        UserAccountDTO dto = new UserAccountDTO();
        dto.setUserAccountId(user.getUserAccountId());
        dto.setFullName(user.getFullName());
        dto.setEmail(user.getEmail());
        dto.setPhoneNumber(user.getPhoneNumber());
        dto.setRole(user.getRole());
        dto.setIsActive(user.getIsActive());
        dto.setProfilePictureUrl(user.getProfilePictureUrl());
        dto.setAddress(user.getAddress());
        dto.setBio(user.getBio());
        dto.setEmailVerified(user.getEmailVerified());
        dto.setPhoneVerified(user.getPhoneVerified());
        dto.setCreatedAt(user.getCreatedAt());
        dto.setLastLoginAt(user.getLastLoginAt());
        return dto;
    }
}