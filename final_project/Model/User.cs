﻿using System.Collections.Generic;
using System;
using CryptSharp;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace final_project.Model
{
	[Table("Users")]
	public class User : Base
	{


		public User() {
			this.CreatedDatetime = DateTime.Now;
		}

		public User(string username, string password, string email) {
			this.Username = username;
			this.Password = password;
			this.Email = email;
			this.CreatedDatetime = DateTime.Now;
		}

		public bool ValidatePassword(string password) 
		{
			if (Crypter.CheckPassword(password, this.Password))
				return true;
			return false;
		}

		[Column(TypeName="VARCHAR")]
		[StringLength(200)]
		[Index("UsernameIndex", 0, IsUnique=true)]
		public string Username { get; set; }


		[Column(TypeName = "VARCHAR")]
		[StringLength(200)]
		[Index("EmailUniqueIndex", 0, IsUnique = true)]
		public string Email { get; set; }

		public ICollection<Order> Order { get; set; }

		[NotMapped]
		public string Password 
		{ 
			get { return PasswordHash; }
			set { PasswordHash = Crypter.Blowfish.Crypt(value); }
		}

		public DateTime CreatedDatetime { get; set; }

		public string PasswordHash { get; set; }

	}
}
