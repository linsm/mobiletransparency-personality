// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

public class User {
    
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; }
}