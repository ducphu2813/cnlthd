﻿namespace APIApplication.DTO.Users;

public class UserDTO
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}