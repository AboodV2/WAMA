﻿namespace Models.POCOs
{
    public enum UserAccountType
    {
        Unknown,
        Patron = 1,
        Employee = 2,
        Manager = 4,
        Administrator = 8,
        Mantainance = 16
    }
}