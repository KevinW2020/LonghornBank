using LonghornBank.Models;
using System.Data.Entity.Migrations;
using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LonghornBank.Migrations
{
	public static class AddCustomers
	{
		// public static AppDbContext db = new AppDbContext();

        public static void SeedCustomers(AppDbContext db)
		{
            UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(db));
            String roleName = "Customer";
            if (roleManager.RoleExists(roleName) == false)
            {
                roleManager.Create(new AppRole(roleName));
            }

			String strEmail1 = "cbaker@freezing.co.uk";
			AppUser c1 = userManager.FindByName(strEmail1);
			if (c1 == null)
			{
  			    c1 = new AppUser()
  			    {
          			Email = strEmail1,
          			FName = "Christopher",
          			LName = "Baker",
          			Initial = "L",
          			StreetAddress = "1245 Lake Austin Blvd.",
          			ZIP = "78733",
          			PhoneNumber = "5125571146",
          			DOB = new DateTime(1991,02,07),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail1
          		};

  			    IdentityResult userResult = userManager.Create(c1, "gazing");
  			    if (userResult.Succeeded)
  			    {
          			var result = userManager.AddToRole(c1.Id, roleName);
  			    }

  		    }
			db.SaveChanges();

			String strEmail2 = "mb@aool.com";
			AppUser c2 = userManager.FindByName(strEmail2);
			if (c2 == null)
			{
  			c2 = new AppUser()
  			{
          			Email = strEmail2,
          			FName = "Michelle",
          			LName = "Banks",
          			Initial = "None",
          			StreetAddress = "1300 Tall Pine Lane",
          			ZIP = "78261",
          			PhoneNumber = "2102678873",
          			DOB = new DateTime(1990,06,23),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail2
          		};

  			IdentityResult userResult = userManager.Create(c2, "banquet");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c2.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail3 = "fd@aool.com";
			AppUser c3 = userManager.FindByName(strEmail3);
			if (c3 == null)
			{
  			c3 = new AppUser()
  			{
          			Email = strEmail3,
          			FName = "Franco",
          			LName = "Broccolo",
          			Initial = "V",
          			StreetAddress = "62 Browning Rd",
          			ZIP = "77019",
          			PhoneNumber = "8175659699",
          			DOB = new DateTime(1986,05,06),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail3
          		};

  			IdentityResult userResult = userManager.Create(c3, "666666");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c3.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail4 = "wendy@ggmail.com";
			AppUser c4 = userManager.FindByName(strEmail4);
			if (c4 == null)
			{
  			c4 = new AppUser()
  			{
          			Email = strEmail4,
          			FName = "Wendy",
          			LName = "Chang",
          			Initial = "L",
          			StreetAddress = "202 Bellmont Hall",
          			ZIP = "78713",
          			PhoneNumber = "5125943222",
          			DOB = new DateTime(1964,12,21),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail4
          		};

  			IdentityResult userResult = userManager.Create(c4, "clover");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c4.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail5 = "limchou@yaho.com";
			AppUser c5 = userManager.FindByName(strEmail5);
			if (c5 == null)
			{
  			c5 = new AppUser()
  			{
          			Email = strEmail5,
          			FName = "Lim",
          			LName = "Chou",
          			Initial = "None",
          			StreetAddress = "1600 Teresa Lane",
          			ZIP = "78266",
          			PhoneNumber = "2107724599",
          			DOB = new DateTime(1950,06,14),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail5
          		};

  			IdentityResult userResult = userManager.Create(c5, "austin");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c5.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail6 = "Dixon@aool.com";
			AppUser c6 = userManager.FindByName(strEmail6);
			if (c6 == null)
			{
  			c6 = new AppUser()
  			{
          			Email = strEmail6,
          			FName = "Shan",
          			LName = "Dixon",
          			Initial = "D",
          			StreetAddress = "234 Holston Circle",
          			ZIP = "75208",
          			PhoneNumber = "2142643255",
          			DOB = new DateTime(1930,05,09),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail6
          		};

  			IdentityResult userResult = userManager.Create(c6, "mailbox");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c6.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail7 = "louann@ggmail.com";
			AppUser c7 = userManager.FindByName(strEmail7);
			if (c7 == null)
			{
  			c7 = new AppUser()
  			{
          			Email = strEmail7,
          			FName = "Lou Ann",
          			LName = "Feeley",
          			Initial = "K",
          			StreetAddress = "600 S 8th Street W",
          			ZIP = "77010",
          			PhoneNumber = "8172556749",
          			DOB = new DateTime(1930,02,24),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail7
          		};

  			IdentityResult userResult = userManager.Create(c7, "aggies");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c7.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail8 = "tfreeley@minntonka.ci.state.mn.us";
			AppUser c8 = userManager.FindByName(strEmail8);
			if (c8 == null)
			{
  			c8 = new AppUser()
  			{
          			Email = strEmail8,
          			FName = "Tesa",
          			LName = "Freeley",
          			Initial = "P",
          			StreetAddress = "4448 Fairview Ave.",
          			ZIP = "77009",
          			PhoneNumber = "8173255687",
          			DOB = new DateTime(1935,09,01),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail8
          		};

  			IdentityResult userResult = userManager.Create(c8, "raiders");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c8.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail9 = "mgar@aool.com";
			AppUser c9 = userManager.FindByName(strEmail9);
			if (c9 == null)
			{
  			c9 = new AppUser()
  			{
          			Email = strEmail9,
          			FName = "Margaret",
          			LName = "Garcia",
          			Initial = "L",
          			StreetAddress = "594 Longview",
          			ZIP = "77003",
          			PhoneNumber = "8176593544",
          			DOB = new DateTime(1990,07,03),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail9
          		};

  			IdentityResult userResult = userManager.Create(c9, "mustangs");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c9.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail10 = "chaley@thug.com";
			AppUser c10 = userManager.FindByName(strEmail10);
			if (c10 == null)
			{
  			c10 = new AppUser()
  			{
          			Email = strEmail10,
          			FName = "Charles",
          			LName = "Haley",
          			Initial = "E",
          			StreetAddress = "One Cowboy Pkwy",
          			ZIP = "75261",
          			PhoneNumber = "2148475583",
          			DOB = new DateTime(1985,09,17),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail10
          		};

  			IdentityResult userResult = userManager.Create(c10, "region");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c10.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail11 = "jeff@ggmail.com";
			AppUser c11 = userManager.FindByName(strEmail11);
			if (c11 == null)
			{
  			c11 = new AppUser()
  			{
          			Email = strEmail11,
          			FName = "Jeffrey",
          			LName = "Hampton",
          			Initial = "T",
          			StreetAddress = "337 38th St.",
          			ZIP = "78705",
          			PhoneNumber = "5126978613",
          			DOB = new DateTime(1995,01,23),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail11
          		};

  			IdentityResult userResult = userManager.Create(c11, "hungry");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c11.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail12 = "wjhearniii@umch.edu";
			AppUser c12 = userManager.FindByName(strEmail12);
			if (c12 == null)
			{
  			c12 = new AppUser()
  			{
          			Email = strEmail12,
          			FName = "John",
          			LName = "Hearn",
          			Initial = "B",
          			StreetAddress = "4225 North First",
          			ZIP = "75237",
          			PhoneNumber = "2148965621",
          			DOB = new DateTime(1994,01,08),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail12
          		};

  			IdentityResult userResult = userManager.Create(c12, "logicon");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c12.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail13 = "hicks43@ggmail.com";
			AppUser c13 = userManager.FindByName(strEmail13);
			if (c13 == null)
			{
  			c13 = new AppUser()
  			{
          			Email = strEmail13,
          			FName = "Anthony",
          			LName = "Hicks",
          			Initial = "J",
          			StreetAddress = "32 NE Garden Ln., Ste 910",
          			ZIP = "78239",
          			PhoneNumber = "2105788965",
          			DOB = new DateTime(1990,10,06),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail13
          		};

  			IdentityResult userResult = userManager.Create(c13, "doofus");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c13.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail14 = "bradsingram@mall.utexas.edu";
			AppUser c14 = userManager.FindByName(strEmail14);
			if (c14 == null)
			{
  			c14 = new AppUser()
  			{
          			Email = strEmail14,
          			FName = "Brad",
          			LName = "Ingram",
          			Initial = "S",
          			StreetAddress = "6548 La Posada Ct.",
          			ZIP = "78736",
          			PhoneNumber = "5124678821",
          			DOB = new DateTime(1984,04,12),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail14
          		};

  			IdentityResult userResult = userManager.Create(c14, "mother");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c14.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail15 = "mother.Ingram@aool.com";
			AppUser c15 = userManager.FindByName(strEmail15);
			if (c15 == null)
			{
  			c15 = new AppUser()
  			{
          			Email = strEmail15,
          			FName = "Todd",
          			LName = "Jacobs",
          			Initial = "L",
          			StreetAddress = "4564 Elm St.",
          			ZIP = "78731",
          			PhoneNumber = "5124653365",
          			DOB = new DateTime(1983,04,04),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail15
          		};

  			IdentityResult userResult = userManager.Create(c15, "whimsical");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c15.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail16 = "victoria@aool.com";
			AppUser c16 = userManager.FindByName(strEmail16);
			if (c16 == null)
			{
  			c16 = new AppUser()
  			{
          			Email = strEmail16,
          			FName = "Victoria",
          			LName = "Lawrence",
          			Initial = "M",
          			StreetAddress = "6639 Butterfly Ln.",
          			ZIP = "78761",
          			PhoneNumber = "5129457399",
          			DOB = new DateTime(1961,02,03),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail16
          		};

  			IdentityResult userResult = userManager.Create(c16, "nothing");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c16.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail17 = "lineback@flush.net";
			AppUser c17 = userManager.FindByName(strEmail17);
			if (c17 == null)
			{
  			c17 = new AppUser()
  			{
          			Email = strEmail17,
          			FName = "Erik",
          			LName = "Lineback",
          			Initial = "W",
          			StreetAddress = "1300 Netherland St",
          			ZIP = "78293",
          			PhoneNumber = "2102449976",
          			DOB = new DateTime(1946,09,03),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail17
          		};

  			IdentityResult userResult = userManager.Create(c17, "GoodFellow");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c17.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail18 = "elowe@netscrape.net";
			AppUser c18 = userManager.FindByName(strEmail18);
			if (c18 == null)
			{
  			c18 = new AppUser()
  			{
          			Email = strEmail18,
          			FName = "Ernest",
          			LName = "Lowe",
          			Initial = "S",
          			StreetAddress = "3201 Pine Drive",
          			ZIP = "78279",
          			PhoneNumber = "2105344627",
          			DOB = new DateTime(1992,02,07),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail18
          		};

  			IdentityResult userResult = userManager.Create(c18, "impede");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c18.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail19 = "luce_chuck@ggmail.com";
			AppUser c19 = userManager.FindByName(strEmail19);
			if (c19 == null)
			{
  			c19 = new AppUser()
  			{
          			Email = strEmail19,
          			FName = "Chuck",
          			LName = "Luce",
          			Initial = "B",
          			StreetAddress = "2345 Rolling Clouds",
          			ZIP = "78268",
          			PhoneNumber = "2106983548",
          			DOB = new DateTime(1942,10,25),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail19
          		};

  			IdentityResult userResult = userManager.Create(c19, "LuceyDucey");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c19.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail20 = "mackcloud@pimpdaddy.com";
			AppUser c20 = userManager.FindByName(strEmail20);
			if (c20 == null)
			{
  			c20 = new AppUser()
  			{
          			Email = strEmail20,
          			FName = "Jennifer",
          			LName = "MacLeod",
          			Initial = "D",
          			StreetAddress = "2504 Far West Blvd.",
          			ZIP = "78731",
          			PhoneNumber = "5124748138",
          			DOB = new DateTime(1965,08,06),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail20
          		};

  			IdentityResult userResult = userManager.Create(c20, "cloudyday");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c20.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail21 = "liz@ggmail.com";
			AppUser c21 = userManager.FindByName(strEmail21);
			if (c21 == null)
			{
  			c21 = new AppUser()
  			{
          			Email = strEmail21,
          			FName = "Elizabeth",
          			LName = "Markham",
          			Initial = "P",
          			StreetAddress = "7861 Chevy Chase",
          			ZIP = "78732",
          			PhoneNumber = "5124579845",
          			DOB = new DateTime(1959,04,13),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail21
          		};

  			IdentityResult userResult = userManager.Create(c21, "emarkbark");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c21.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail22 = "mclarence@aool.com";
			AppUser c22 = userManager.FindByName(strEmail22);
			if (c22 == null)
			{
  			c22 = new AppUser()
  			{
          			Email = strEmail22,
          			FName = "Clarence",
          			LName = "Martin",
          			Initial = "A",
          			StreetAddress = "87 Alcedo St.",
          			ZIP = "77045",
          			PhoneNumber = "8174955201",
          			DOB = new DateTime(1990,01,06),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail22
          		};

  			IdentityResult userResult = userManager.Create(c22, "smartinmartin");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c22.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail23 = "smartinmartin.Martin@aool.com";
			AppUser c23 = userManager.FindByName(strEmail23);
			if (c23 == null)
			{
  			c23 = new AppUser()
  			{
          			Email = strEmail23,
          			FName = "Gregory",
          			LName = "Martinez",
          			Initial = "R",
          			StreetAddress = "8295 Sunset Blvd.",
          			ZIP = "77030",
          			PhoneNumber = "8178746718",
          			DOB = new DateTime(1987,10,09),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail23
          		};

  			IdentityResult userResult = userManager.Create(c23, "looter");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c23.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail24 = "cmiller@mapster.com";
			AppUser c24 = userManager.FindByName(strEmail24);
			if (c24 == null)
			{
  			c24 = new AppUser()
  			{
          			Email = strEmail24,
          			FName = "Charles",
          			LName = "Miller",
          			Initial = "R",
          			StreetAddress = "8962 Main St.",
          			ZIP = "77031",
          			PhoneNumber = "8177458615",
          			DOB = new DateTime(1984,07,21),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail24
          		};

  			IdentityResult userResult = userManager.Create(c24, "chucky33");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c24.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail25 = "nelson.Kelly@aool.com";
			AppUser c25 = userManager.FindByName(strEmail25);
			if (c25 == null)
			{
  			c25 = new AppUser()
  			{
          			Email = strEmail25,
          			FName = "Kelly",
          			LName = "Nelson",
          			Initial = "T",
          			StreetAddress = "2601 Red River",
          			ZIP = "78703",
          			PhoneNumber = "5122926966",
          			DOB = new DateTime(1956,07,04),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail25
          		};

  			IdentityResult userResult = userManager.Create(c25, "orange");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c25.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail26 = "jojoe@ggmail.com";
			AppUser c26 = userManager.FindByName(strEmail26);
			if (c26 == null)
			{
  			c26 = new AppUser()
  			{
          			Email = strEmail26,
          			FName = "Joe",
          			LName = "Nguyen",
          			Initial = "C",
          			StreetAddress = "1249 4th SW St.",
          			ZIP = "75238",
          			PhoneNumber = "2143125897",
          			DOB = new DateTime(1963,01,29),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail26
          		};

  			IdentityResult userResult = userManager.Create(c26, "victorious");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c26.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail27 = "orielly@foxnets.com";
			AppUser c27 = userManager.FindByName(strEmail27);
			if (c27 == null)
			{
  			c27 = new AppUser()
  			{
          			Email = strEmail27,
          			FName = "Bill",
          			LName = "O'Reilly",
          			Initial = "T",
          			StreetAddress = "8800 Gringo Drive",
          			ZIP = "78260",
          			PhoneNumber = "2103450925",
          			DOB = new DateTime(1983,01,07),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail27
          		};

  			IdentityResult userResult = userManager.Create(c27, "billyboy");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c27.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail28 = "or@aool.com";
			AppUser c28 = userManager.FindByName(strEmail28);
			if (c28 == null)
			{
  			c28 = new AppUser()
  			{
          			Email = strEmail28,
          			FName = "Anka",
          			LName = "Radkovich",
          			Initial = "L",
          			StreetAddress = "1300 Elliott Pl",
          			ZIP = "75260",
          			PhoneNumber = "2142345566",
          			DOB = new DateTime(1980,03,31),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail28
          		};

  			IdentityResult userResult = userManager.Create(c28, "radicalone");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c28.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail29 = "megrhodes@freezing.co.uk";
			AppUser c29 = userManager.FindByName(strEmail29);
			if (c29 == null)
			{
  			c29 = new AppUser()
  			{
          			Email = strEmail29,
          			FName = "Megan",
          			LName = "Rhodes",
          			Initial = "C",
          			StreetAddress = "4587 Enfield Rd.",
          			ZIP = "78707",
          			PhoneNumber = "5123744746",
          			DOB = new DateTime(1944,08,12),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail29
          		};

  			IdentityResult userResult = userManager.Create(c29, "gohorns");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c29.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail30 = "erynrice@aool.com";
			AppUser c30 = userManager.FindByName(strEmail30);
			if (c30 == null)
			{
  			c30 = new AppUser()
  			{
          			Email = strEmail30,
          			FName = "Eryn",
          			LName = "Rice",
          			Initial = "M",
          			StreetAddress = "3405 Rio Grande",
          			ZIP = "78705",
          			PhoneNumber = "5123876657",
          			DOB = new DateTime(1934,08,02),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail30
          		};

  			IdentityResult userResult = userManager.Create(c30, "iloveme");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c30.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail31 = "jorge@hootmail.com";
			AppUser c31 = userManager.FindByName(strEmail31);
			if (c31 == null)
			{
  			c31 = new AppUser()
  			{
          			Email = strEmail31,
          			FName = "Jorge",
          			LName = "Rodriguez",
          			Initial = "None",
          			StreetAddress = "6788 Cotter Street",
          			ZIP = "77057",
          			PhoneNumber = "8178904374",
          			DOB = new DateTime(1989,08,11),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail31
          		};

  			IdentityResult userResult = userManager.Create(c31, "greedy");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c31.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail32 = "ra@aoo.com";
			AppUser c32 = userManager.FindByName(strEmail32);
			if (c32 == null)
			{
  			c32 = new AppUser()
  			{
          			Email = strEmail32,
          			FName = "Allen",
          			LName = "Rogers",
          			Initial = "B",
          			StreetAddress = "4965 Oak Hill",
          			ZIP = "78732",
          			PhoneNumber = "5128752943",
          			DOB = new DateTime(1967,08,27),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail32
          		};

  			IdentityResult userResult = userManager.Create(c32, "familiar");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c32.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail33 = "stjean@home.com";
			AppUser c33 = userManager.FindByName(strEmail33);
			if (c33 == null)
			{
  			c33 = new AppUser()
  			{
          			Email = strEmail33,
          			FName = "Olivier",
          			LName = "Saint-Jean",
          			Initial = "M",
          			StreetAddress = "255 Toncray Dr.",
          			ZIP = "78292",
          			PhoneNumber = "2104145678",
          			DOB = new DateTime(1950,07,08),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail33
          		};

  			IdentityResult userResult = userManager.Create(c33, "historical");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c33.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail34 = "ss34@ggmail.com";
			AppUser c34 = userManager.FindByName(strEmail34);
			if (c34 == null)
			{
  			c34 = new AppUser()
  			{
          			Email = strEmail34,
          			FName = "Sarah",
          			LName = "Saunders",
          			Initial = "J",
          			StreetAddress = "332 Avenue C",
          			ZIP = "78705",
          			PhoneNumber = "5123497810",
          			DOB = new DateTime(1977,10,29),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail34
          		};

  			IdentityResult userResult = userManager.Create(c34, "guiltless");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c34.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail35 = "willsheff@email.com";
			AppUser c35 = userManager.FindByName(strEmail35);
			if (c35 == null)
			{
  			c35 = new AppUser()
  			{
          			Email = strEmail35,
          			FName = "William",
          			LName = "Sewell",
          			Initial = "T",
          			StreetAddress = "2365 51st St.",
          			ZIP = "78709",
          			PhoneNumber = "5124510084",
          			DOB = new DateTime(1941,04,21),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail35
          		};

  			IdentityResult userResult = userManager.Create(c35, "frequent");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c35.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail36 = "sheff44@ggmail.com";
			AppUser c36 = userManager.FindByName(strEmail36);
			if (c36 == null)
			{
  			c36 = new AppUser()
  			{
          			Email = strEmail36,
          			FName = "Martin",
          			LName = "Sheffield",
          			Initial = "J",
          			StreetAddress = "3886 Avenue A",
          			ZIP = "78705",
          			PhoneNumber = "5125479167",
          			DOB = new DateTime(1937,11,10),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail36
          		};

  			IdentityResult userResult = userManager.Create(c36, "history");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c36.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail37 = "johnsmith187@aool.com";
			AppUser c37 = userManager.FindByName(strEmail37);
			if (c37 == null)
			{
  			c37 = new AppUser()
  			{
          			Email = strEmail37,
          			FName = "John",
          			LName = "Smith",
          			Initial = "A",
          			StreetAddress = "23 Hidden Forge Dr.",
          			ZIP = "78280",
          			PhoneNumber = "2108321888",
          			DOB = new DateTime(1954,10,26),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail37
          		};

  			IdentityResult userResult = userManager.Create(c37, "squirrel");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c37.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail38 = "dustroud@mail.com";
			AppUser c38 = userManager.FindByName(strEmail38);
			if (c38 == null)
			{
  			c38 = new AppUser()
  			{
          			Email = strEmail38,
          			FName = "Dustin",
          			LName = "Stroud",
          			Initial = "P",
          			StreetAddress = "1212 Rita Rd",
          			ZIP = "75221",
          			PhoneNumber = "2142346667",
          			DOB = new DateTime(1932,09,01),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail38
          		};

  			IdentityResult userResult = userManager.Create(c38, "snakes");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c38.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail39 = "ericstuart@aool.com";
			AppUser c39 = userManager.FindByName(strEmail39);
			if (c39 == null)
			{
  			c39 = new AppUser()
  			{
          			Email = strEmail39,
          			FName = "Eric",
          			LName = "Stuart",
          			Initial = "D",
          			StreetAddress = "5576 Toro Ring",
          			ZIP = "78746",
          			PhoneNumber = "5128178335",
          			DOB = new DateTime(1930,12,28),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail39
          		};

  			IdentityResult userResult = userManager.Create(c39, "landus");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c39.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail40 = "peterstump@hootmail.com";
			AppUser c40 = userManager.FindByName(strEmail40);
			if (c40 == null)
			{
  			c40 = new AppUser()
  			{
          			Email = strEmail40,
          			FName = "Peter",
          			LName = "Stump",
          			Initial = "L",
          			StreetAddress = "1300 Kellen Circle",
          			ZIP = "77018",
          			PhoneNumber = "8174560903",
          			DOB = new DateTime(1989,08,13),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail40
          		};

  			IdentityResult userResult = userManager.Create(c40, "rhythm");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c40.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail41 = "tanner@ggmail.com";
			AppUser c41 = userManager.FindByName(strEmail41);
			if (c41 == null)
			{
  			c41 = new AppUser()
  			{
          			Email = strEmail41,
          			FName = "Jeremy",
          			LName = "Tanner",
          			Initial = "S",
          			StreetAddress = "4347 Almstead",
          			ZIP = "77044",
          			PhoneNumber = "8174590929",
          			DOB = new DateTime(1982,05,21),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail41
          		};

  			IdentityResult userResult = userManager.Create(c41, "kindly");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c41.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail42 = "taylordjay@aool.com";
			AppUser c42 = userManager.FindByName(strEmail42);
			if (c42 == null)
			{
  			c42 = new AppUser()
  			{
          			Email = strEmail42,
          			FName = "Allison",
          			LName = "Taylor",
          			Initial = "R",
          			StreetAddress = "467 Nueces St.",
          			ZIP = "78705",
          			PhoneNumber = "5124748452",
          			DOB = new DateTime(1960,01,08),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail42
          		};

  			IdentityResult userResult = userManager.Create(c42, "instrument");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c42.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail43 = "TayTaylor@aool.com";
			AppUser c43 = userManager.FindByName(strEmail43);
			if (c43 == null)
			{
  			c43 = new AppUser()
  			{
          			Email = strEmail43,
          			FName = "Rachel",
          			LName = "Taylor",
          			Initial = "K",
          			StreetAddress = "345 Longview Dr.",
          			ZIP = "78705",
          			PhoneNumber = "5124512631",
          			DOB = new DateTime(1975,07,27),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail43
          		};

  			IdentityResult userResult = userManager.Create(c43, "arched");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c43.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail44 = "teefrank@hootmail.com";
			AppUser c44 = userManager.FindByName(strEmail44);
			if (c44 == null)
			{
  			c44 = new AppUser()
  			{
          			Email = strEmail44,
          			FName = "Frank",
          			LName = "Tee",
          			Initial = "J",
          			StreetAddress = "5590 Lavell Dr",
          			ZIP = "77004",
          			PhoneNumber = "8178765543",
          			DOB = new DateTime(1968,04,06),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail44
          		};

  			IdentityResult userResult = userManager.Create(c44, "median");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c44.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail45 = "tuck33@ggmail.com";
			AppUser c45 = userManager.FindByName(strEmail45);
			if (c45 == null)
			{
  			c45 = new AppUser()
  			{
          			Email = strEmail45,
          			FName = "Clent",
          			LName = "Tucker",
          			Initial = "J",
          			StreetAddress = "312 Main St.",
          			ZIP = "75315",
          			PhoneNumber = "2148471154",
          			DOB = new DateTime(1978,05,19),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail45
          		};

  			IdentityResult userResult = userManager.Create(c45, "approval");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c45.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail46 = "avelasco@yaho.com";
			AppUser c46 = userManager.FindByName(strEmail46);
			if (c46 == null)
			{
  			c46 = new AppUser()
  			{
          			Email = strEmail46,
          			FName = "Allen",
          			LName = "Velasco",
          			Initial = "G",
          			StreetAddress = "679 W. 4th",
          			ZIP = "75207",
          			PhoneNumber = "2143985638",
          			DOB = new DateTime(1963,10,06),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail46
          		};

  			IdentityResult userResult = userManager.Create(c46, "decorate");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c46.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail47 = "westj@pioneer.net";
			AppUser c47 = userManager.FindByName(strEmail47);
			if (c47 == null)
			{
  			c47 = new AppUser()
  			{
          			Email = strEmail47,
          			FName = "Jake",
          			LName = "West",
          			Initial = "T",
          			StreetAddress = "RR 3287",
          			ZIP = "75323",
          			PhoneNumber = "2148475244",
          			DOB = new DateTime(1993,10,14),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail47
          		};

  			IdentityResult userResult = userManager.Create(c47, "grover");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c47.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail48 = "louielouie@aool.com";
			AppUser c48 = userManager.FindByName(strEmail48);
			if (c48 == null)
			{
  			c48 = new AppUser()
  			{
          			Email = strEmail48,
          			FName = "Louis",
          			LName = "Winthorpe",
          			Initial = "L",
          			StreetAddress = "2500 Padre Blvd",
          			ZIP = "75220",
          			PhoneNumber = "2145650098",
          			DOB = new DateTime(1952,05,31),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail48
          		};

  			IdentityResult userResult = userManager.Create(c48, "sturdy");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c48.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail49 = "rwood@voyager.net";
			AppUser c49 = userManager.FindByName(strEmail49);
			if (c49 == null)
			{
  			c49 = new AppUser()
  			{
          			Email = strEmail49,
          			FName = "Reagan",
          			LName = "Wood",
          			Initial = "B",
          			StreetAddress = "447 Westlake Dr.",
          			ZIP = "78746",
          			PhoneNumber = "5124545242",
          			DOB = new DateTime(1992,04,24),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail49
          		};

  			IdentityResult userResult = userManager.Create(c49, "decorous");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c49.Id, roleName);
  			}

  		}
			db.SaveChanges();

        }

        public static void SeedEmployees(AppDbContext db)
		{
			UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
			AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(db));
			String roleName = "Employee";
			if (roleManager.RoleExists(roleName) == false)
			{
  			roleManager.Create(new AppRole(roleName));
			}

			String roleName2 = "Manager";
			if (roleManager.RoleExists(roleName2) == false)
			{
  			roleManager.Create(new AppRole(roleName2));
			}

			String strEmail1 = "t.jacobs@longhornbank.neet";
			AppUser c1 = userManager.FindByName(strEmail1);
			if (c1 == null)
			{
  			c1 = new AppUser()
  			{
          			Email = strEmail1,
          			FName = "Todd",
          			LName = "Jacobs",
          			Initial = "L",
          			StreetAddress = "4564 Elm St.",
          			ZIP = "77003",
          			PhoneNumber = "8176593544",
          			DOB = new DateTime(1900,01,01),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail1
          		};

  			IdentityResult userResult = userManager.Create(c1, "society");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c1.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail2 = "e.rice@longhornbank.neet";
			AppUser c2 = userManager.FindByName(strEmail2);
			if (c2 == null)
			{
  			c2 = new AppUser()
  			{
          			Email = strEmail2,
          			FName = "Eryn",
          			LName = "Rice",
          			Initial = "M",
          			StreetAddress = "3405 Rio Grande",
          			ZIP = "75261",
          			PhoneNumber = "2148475583",
          			DOB = new DateTime(1900,01,01),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail2
          		};

  			IdentityResult userResult = userManager.Create(c2, "ricearoni");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c2.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail3 = "b.ingram@longhornbank.neet";
			AppUser c3 = userManager.FindByName(strEmail3);
			if (c3 == null)
			{
  			c3 = new AppUser()
  			{
          			Email = strEmail3,
          			FName = "Brad",
          			LName = "Ingram",
          			Initial = "S",
          			StreetAddress = "6548 La Posada Ct.",
          			ZIP = "78705",
          			PhoneNumber = "5126978613",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail3
          		};

  			IdentityResult userResult = userManager.Create(c3, "ingram45");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c3.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail4 = "a.taylor@longhornbank.neet";
			AppUser c4 = userManager.FindByName(strEmail4);
			if (c4 == null)
			{
  			c4 = new AppUser()
  			{
          			Email = strEmail4,
          			FName = "Allison",
          			LName = "Taylor",
          			Initial = "R",
          			StreetAddress = "467 Nueces St.",
          			ZIP = "75237",
          			PhoneNumber = "2148965621",
          			DOB = new DateTime(1900,01,01),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail4
          		};

  			IdentityResult userResult = userManager.Create(c4, "nostalgic");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c4.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail5 = "g.martinez@longhornbank.neet";
			AppUser c5 = userManager.FindByName(strEmail5);
			if (c5 == null)
			{
  			c5 = new AppUser()
  			{
          			Email = strEmail5,
          			FName = "Gregory",
          			LName = "Martinez",
          			Initial = "R",
          			StreetAddress = "8295 Sunset Blvd.",
          			ZIP = "78239",
          			PhoneNumber = "2105788965",
          			DOB = new DateTime(1900,01,01),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail5
          		};

  			IdentityResult userResult = userManager.Create(c5, "fungus");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c5.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail6 = "m.sheffield@longhornbank.neet";
			AppUser c6 = userManager.FindByName(strEmail6);
			if (c6 == null)
			{
  			c6 = new AppUser()
  			{
          			Email = strEmail6,
          			FName = "Martin",
          			LName = "Sheffield",
          			Initial = "J",
          			StreetAddress = "3886 Avenue A",
          			ZIP = "78736",
          			PhoneNumber = "5124678821",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail6
          		};

  			IdentityResult userResult = userManager.Create(c6, "longhorns");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c6.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail7 = "j.macleod@longhornbank.neet";
			AppUser c7 = userManager.FindByName(strEmail7);
			if (c7 == null)
			{
  			c7 = new AppUser()
  			{
          			Email = strEmail7,
          			FName = "Jennifer",
          			LName = "MacLeod",
          			Initial = "D",
          			StreetAddress = "2504 Far West Blvd.",
          			ZIP = "78731",
          			PhoneNumber = "5124653365",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail7
          		};

  			IdentityResult userResult = userManager.Create(c7, "smitty");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c7.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail8 = "j.tanner@longhornbank.neet";
			AppUser c8 = userManager.FindByName(strEmail8);
			if (c8 == null)
			{
  			c8 = new AppUser()
  			{
          			Email = strEmail8,
          			FName = "Jeremy",
          			LName = "Tanner",
          			Initial = "S",
          			StreetAddress = "4347 Almstead",
          			ZIP = "78761",
          			PhoneNumber = "5129457399",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail8
          		};

  			IdentityResult userResult = userManager.Create(c8, "tanman");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c8.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail9 = "m.rhodes@longhornbank.neet";
			AppUser c9 = userManager.FindByName(strEmail9);
			if (c9 == null)
			{
  			c9 = new AppUser()
  			{
          			Email = strEmail9,
          			FName = "Megan",
          			LName = "Rhodes",
          			Initial = "C",
          			StreetAddress = "4587 Enfield Rd.",
          			ZIP = "78293",
          			PhoneNumber = "2102449976",
          			DOB = new DateTime(1900,01,01),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail9
          		};

  			IdentityResult userResult = userManager.Create(c9, "countryrhodes");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c9.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail10 = "e.stuart@longhornbank.neet";
			AppUser c10 = userManager.FindByName(strEmail10);
			if (c10 == null)
			{
  			c10 = new AppUser()
  			{
          			Email = strEmail10,
          			FName = "Eric",
          			LName = "Stuart",
          			Initial = "F",
          			StreetAddress = "5576 Toro Ring",
          			ZIP = "78279",
          			PhoneNumber = "2105344627",
          			DOB = new DateTime(1900,01,01),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail10
          		};

  			IdentityResult userResult = userManager.Create(c10, "stewboy");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c10.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail11 = "l.chung@longhornbank.neet";
			AppUser c11 = userManager.FindByName(strEmail11);
			if (c11 == null)
			{
  			c11 = new AppUser()
  			{
          			Email = strEmail11,
          			FName = "Lisa",
          			LName = "Chung",
          			Initial = "N",
          			StreetAddress = "234 RR 12",
          			ZIP = "78268",
          			PhoneNumber = "2106983548",
          			DOB = new DateTime(1900,01,01),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail11
          		};

  			IdentityResult userResult = userManager.Create(c11, "lisssa");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c11.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail12 = "l.swanson@longhornbank.neet";
			AppUser c12 = userManager.FindByName(strEmail12);
			if (c12 == null)
			{
  			c12 = new AppUser()
  			{
          			Email = strEmail12,
          			FName = "Leon",
          			LName = "Swanson",
          			Initial = "None",
          			StreetAddress = "245 River Rd",
          			ZIP = "78731",
          			PhoneNumber = "5124748138",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail12
          		};

  			IdentityResult userResult = userManager.Create(c12, "swansong");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c12.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail13 = "w.loter@longhornbank.neet";
			AppUser c13 = userManager.FindByName(strEmail13);
			if (c13 == null)
			{
  			c13 = new AppUser()
  			{
          			Email = strEmail13,
          			FName = "Wanda",
          			LName = "Loter",
          			Initial = "K",
          			StreetAddress = "3453 RR 3235",
          			ZIP = "78732",
          			PhoneNumber = "5124579845",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail13
          		};

  			IdentityResult userResult = userManager.Create(c13, "lottery");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c13.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail14 = "j.white@longhornbank.neet";
			AppUser c14 = userManager.FindByName(strEmail14);
			if (c14 == null)
			{
  			c14 = new AppUser()
  			{
          			Email = strEmail14,
          			FName = "Jason",
          			LName = "White",
          			Initial = "M",
          			StreetAddress = "12 Valley View",
          			ZIP = "77045",
          			PhoneNumber = "8174955201",
          			DOB = new DateTime(1900,01,01),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail14
          		};

  			IdentityResult userResult = userManager.Create(c14, "evanescent");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c14.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail15 = "w.montgomery@longhornbank.neet";
			AppUser c15 = userManager.FindByName(strEmail15);
			if (c15 == null)
			{
  			c15 = new AppUser()
  			{
          			Email = strEmail15,
          			FName = "Wilda",
          			LName = "Montgomery",
          			Initial = "K",
          			StreetAddress = "210 Blanco Dr",
          			ZIP = "77030",
          			PhoneNumber = "8178746718",
          			DOB = new DateTime(1900,01,01),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail15
          		};

  			IdentityResult userResult = userManager.Create(c15, "monty3");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c15.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail16 = "h.morales@longhornbank.neet";
			AppUser c16 = userManager.FindByName(strEmail16);
			if (c16 == null)
			{
  			c16 = new AppUser()
  			{
          			Email = strEmail16,
          			FName = "Hector",
          			LName = "Morales",
          			Initial = "N",
          			StreetAddress = "4501 RR 140",
          			ZIP = "77031",
          			PhoneNumber = "8177458615",
          			DOB = new DateTime(1900,01,01),
          			City = "HOUSTON",
          			State = StateAbbr.TX,
          			UserName = strEmail16
          		};

  			IdentityResult userResult = userManager.Create(c16, "hecktour");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c16.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail17 = "m.rankin@longhornbank.neet";
			AppUser c17 = userManager.FindByName(strEmail17);
			if (c17 == null)
			{
  			c17 = new AppUser()
  			{
          			Email = strEmail17,
          			FName = "Mary",
          			LName = "Rankin",
          			Initial = "T",
          			StreetAddress = "340 Second St",
          			ZIP = "78703",
          			PhoneNumber = "5122926966",
          			DOB = new DateTime(1900,01,01),
          			City = "AUSTIN",
          			State = StateAbbr.TX,
          			UserName = strEmail17
          		};

  			IdentityResult userResult = userManager.Create(c17, "rankmary");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c17.Id, roleName);
  			}

  		}
			db.SaveChanges();

			String strEmail18 = "l.walker@longhornbank.neet";
			AppUser c18 = userManager.FindByName(strEmail18);
			if (c18 == null)
			{
  			c18 = new AppUser()
  			{
          			Email = strEmail18,
          			FName = "Larry",
          			LName = "Walker",
          			Initial = "G",
          			StreetAddress = "9 Bison Circle",
          			ZIP = "75238",
          			PhoneNumber = "2143125897",
          			DOB = new DateTime(1900,01,01),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail18
          		};

  			IdentityResult userResult = userManager.Create(c18, "walkamile");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c18.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail19 = "g.chang@longhornbank.neet";
			AppUser c19 = userManager.FindByName(strEmail19);
			if (c19 == null)
			{
  			c19 = new AppUser()
  			{
          			Email = strEmail19,
          			FName = "George",
          			LName = "Chang",
          			Initial = "M",
          			StreetAddress = "9003 Joshua St",
          			ZIP = "78260",
          			PhoneNumber = "2103450925",
          			DOB = new DateTime(1900,01,01),
          			City = "SAN ANTONIO",
          			State = StateAbbr.TX,
          			UserName = strEmail19
          		};

  			IdentityResult userResult = userManager.Create(c19, "changalang");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c19.Id, roleName2);
  			}

  		}
			db.SaveChanges();

			String strEmail20 = "g.gonzalez@longhornbank.neet";
			AppUser c20 = userManager.FindByName(strEmail20);
			if (c20 == null)
			{
  			c20 = new AppUser()
  			{
          			Email = strEmail20,
          			FName = "Gwen",
          			LName = "Gonzalez",
          			Initial = "J",
          			StreetAddress = "103 Manor Rd",
          			ZIP = "75260",
          			PhoneNumber = "2142345566",
          			DOB = new DateTime(1900,01,01),
          			City = "DALLAS",
          			State = StateAbbr.TX,
          			UserName = strEmail20
          		};

  			IdentityResult userResult = userManager.Create(c20, "offbeat");
  			if (userResult.Succeeded)
  			{
          			var result = userManager.AddToRole(c20.Id, roleName);
  			}

  		}
			db.SaveChanges();

        }

        public static void SeedAccounts(AppDbContext db)
		{
			StockPortfolio c1 = new StockPortfolio();
			c1.StockPortfolioNumber = 1000000000;
			c1.StockPortfolioName = "Shan's Stock";
            c1.CashBalance = 0m;
            c1.StockBalance = 0m;
			db.StockPortfolios.AddOrUpdate(c => c.StockPortfolioNumber, c1);
			c1.AppUser = db.Users.SingleOrDefault(s => s.Email == "Dixon@aool.com");
			db.SaveChanges();

			Savings c2 = new Savings();
			c2.SavingsNumber = 1000000001;
			c2.SavingsName = "William's Savings";
			c2.SavingsBalance = 40035.5m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c2);
			c2.AppUser = db.Users.SingleOrDefault(s => s.Email == "willsheff@email.com");
			db.SaveChanges();

			Checking c3 = new Checking();
			c3.CheckingNumber = 1000000002;
			c3.CheckingName = "Gregory's Checking";
			c3.CheckingBalance = 39779.49m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c3);
			c3.AppUser = db.Users.SingleOrDefault(s => s.Email == "smartinmartin.Martin@aool.com");
			db.SaveChanges();

			Checking c4 = new Checking();
			c4.CheckingNumber = 1000000003;
			c4.CheckingName = "Allen's Checking";
			c4.CheckingBalance = 47277.33m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c4);
			c4.AppUser = db.Users.SingleOrDefault(s => s.Email == "avelasco@yaho.com");
			db.SaveChanges();

			Checking c5 = new Checking();
			c5.CheckingNumber = 1000000004;
			c5.CheckingName = "Reagan's Checking";
			c5.CheckingBalance = 70812.15m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c5);
			c5.AppUser = db.Users.SingleOrDefault(s => s.Email == "rwood@voyager.net");
			db.SaveChanges();

			Savings c6 = new Savings();
			c6.SavingsNumber = 1000000005;
			c6.SavingsName = "Kelly's Savings";
			c6.SavingsBalance = 21901.97m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c6);
			c6.AppUser = db.Users.SingleOrDefault(s => s.Email == "nelson.Kelly@aool.com");
			db.SaveChanges();

			Checking c7 = new Checking();
			c7.CheckingNumber = 1000000006;
			c7.CheckingName = "Eryn's Checking";
			c7.CheckingBalance = 70480.99m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c7);
			c7.AppUser = db.Users.SingleOrDefault(s => s.Email == "erynrice@aool.com");
			db.SaveChanges();

			Savings c8 = new Savings();
			c8.SavingsNumber = 1000000007;
			c8.SavingsName = "Jake's Savings";
			c8.SavingsBalance = 7916.4m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c8);
			c8.AppUser = db.Users.SingleOrDefault(s => s.Email == "westj@pioneer.net");
			db.SaveChanges();

			StockPortfolio c9 = new StockPortfolio();
			c9.StockPortfolioNumber = 1000000008;
			c9.StockPortfolioName = "Michelle's Stock";
            c9.CashBalance = 0m;
            c9.StockBalance = 0m;
            db.StockPortfolios.AddOrUpdate(c => c.StockPortfolioNumber, c9);
			c9.AppUser = db.Users.SingleOrDefault(s => s.Email == "mb@aool.com");
			db.SaveChanges();

			Savings c10 = new Savings();
			c10.SavingsNumber = 1000000009;
			c10.SavingsName = "Jeffrey's Savings";
			c10.SavingsBalance = 69576.83m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c10);
			c10.AppUser = db.Users.SingleOrDefault(s => s.Email == "jeff@ggmail.com");
			db.SaveChanges();

			StockPortfolio c11 = new StockPortfolio();
			c11.StockPortfolioNumber = 1000000010;
			c11.StockPortfolioName = "Kelly's Stock";
            c11.CashBalance = 0m;
            c11.StockBalance = 0m;
            db.StockPortfolios.AddOrUpdate(c => c.StockPortfolioNumber, c11);
			c11.AppUser = db.Users.SingleOrDefault(s => s.Email == "nelson.Kelly@aool.com");
			db.SaveChanges();

			Checking c12 = new Checking();
			c12.CheckingNumber = 1000000011;
			c12.CheckingName = "Eryn's Checking 2";
			c12.CheckingBalance = 30279.33m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c12);
			c12.AppUser = db.Users.SingleOrDefault(s => s.Email == "erynrice@aool.com");
			db.SaveChanges();

			IRA c13 = new IRA();
			c13.IRANumber = 1000000012;
			c13.IRAName = "Jennifer's IRA";
			c13.IRABalance = 5000.00m;
			db.IRAs.AddOrUpdate(c => c.IRANumber, c13);
			c13.AppUser = db.Users.SingleOrDefault(s => s.Email == "mackcloud@pimpdaddy.com");
			db.SaveChanges();

			Savings c14 = new Savings();
			c14.SavingsNumber = 1000000013;
			c14.SavingsName = "Sarah's Savings";
			c14.SavingsBalance = 11958.08m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c14);
			c14.AppUser = db.Users.SingleOrDefault(s => s.Email == "ss34@ggmail.com");
			db.SaveChanges();

			Savings c15 = new Savings();
			c15.SavingsNumber = 1000000014;
			c15.SavingsName = "Jeremy's Savings";
			c15.SavingsBalance = 72990.47m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c15);
			c15.AppUser = db.Users.SingleOrDefault(s => s.Email == "tanner@ggmail.com");
			db.SaveChanges();

			Savings c16 = new Savings();
			c16.SavingsNumber = 1000000015;
			c16.SavingsName = "Elizabeth's Savings";
			c16.SavingsBalance = 7417.2m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c16);
			c16.AppUser = db.Users.SingleOrDefault(s => s.Email == "liz@ggmail.com");
			db.SaveChanges();

			IRA c17 = new IRA();
			c17.IRANumber = 1000000016;
			c17.IRAName = "Allen's IRA";
			c17.IRABalance = 5000.00m;
			db.IRAs.AddOrUpdate(c => c.IRANumber, c17);
			c17.AppUser = db.Users.SingleOrDefault(s => s.Email == "ra@aoo.com");
			db.SaveChanges();

			StockPortfolio c18 = new StockPortfolio();
			c18.StockPortfolioNumber = 1000000017;
			c18.StockPortfolioName = "John's Stock";
            c18.CashBalance = 0m;
            c18.StockBalance = 0m;
            db.StockPortfolios.AddOrUpdate(c => c.StockPortfolioNumber, c18);
			c18.AppUser = db.Users.SingleOrDefault(s => s.Email == "johnsmith187@aool.com");
			db.SaveChanges();

			Savings c19 = new Savings();
			c19.SavingsNumber = 1000000018;
			c19.SavingsName = "Clarence's Savings";
			c19.SavingsBalance = 1642.82m;
			db.Savings.AddOrUpdate(c => c.SavingsNumber, c19);
			c19.AppUser = db.Users.SingleOrDefault(s => s.Email == "mclarence@aool.com");
			db.SaveChanges();

			Checking c20 = new Checking();
			c20.CheckingNumber = 1000000019;
			c20.CheckingName = "Sarah's Checking";
			c20.CheckingBalance = 84421.45m;
			db.Checkings.AddOrUpdate(c => c.CheckingNumber, c20);
			c20.AppUser = db.Users.SingleOrDefault(s => s.Email == "ss34@ggmail.com");
			db.SaveChanges();

        }

        public static void SeedTransactions(AppDbContext db)
        {
            Transaction c2 = new Transaction();
            c2.TypeOfTransaction = 0;
            c2.TransactionDate = new DateTime(2016, 01, 01);
            c2.TransactionDescription = "Initial Deposit";
            c2.TransactionAmount = 40035.50m;
            c2.AccountSender = 1000000001;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c2);
            c2.AppUser = db.Users.SingleOrDefault(s => s.Email == "willsheff@email.com");
            db.SaveChanges();

            Transaction c3 = new Transaction();
            c3.TypeOfTransaction = 0;
            c3.TransactionDate = new DateTime(2016, 01, 01);
            c3.TransactionDescription = "Initial Deposit";
            c3.TransactionAmount = 39779.49m;
            c3.AccountSender = 1000000002;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c3);
            c3.AppUser = db.Users.SingleOrDefault(s => s.Email == "smartinmartin.Martin@aool.com");
            db.SaveChanges();

            Transaction c4 = new Transaction();
            c4.TypeOfTransaction = 0;
            c4.TransactionDate = new DateTime(2016, 01, 01);
            c4.TransactionDescription = "Initial Deposit";
            c4.TransactionAmount = 47277.33m;
            c4.AccountSender = 1000000003;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c4);
            c4.AppUser = db.Users.SingleOrDefault(s => s.Email == "avelasco@yaho.com");
            db.SaveChanges();

            Transaction c5 = new Transaction();
            c5.TypeOfTransaction = 0;
            c5.TransactionDate = new DateTime(2016, 01, 01);
            c5.TransactionDescription = "Initial Deposit";
            c5.TransactionAmount = 70812.15m;
            c5.AccountSender = 1000000004;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c5);
            c5.AppUser = db.Users.SingleOrDefault(s => s.Email == "rwood@voyager.net");
            db.SaveChanges();

            Transaction c6 = new Transaction();
            c6.TypeOfTransaction = 0;
            c6.TransactionDate = new DateTime(2016, 01, 01);
            c6.TransactionDescription = "Initial Deposit";
            c6.TransactionAmount = 21901.97m;
            c6.AccountSender = 1000000005;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c6);
            c6.AppUser = db.Users.SingleOrDefault(s => s.Email == "nelson.Kelly@aool.com");
            db.SaveChanges();

            Transaction c7 = new Transaction();
            c7.TypeOfTransaction = 0;
            c7.TransactionDate = new DateTime(2016, 01, 01);
            c7.TransactionDescription = "Initial Deposit";
            c7.TransactionAmount = 70480.99m;
            c7.AccountSender = 1000000006;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c7);
            c7.AppUser = db.Users.SingleOrDefault(s => s.Email == "erynrice@aool.com");
            db.SaveChanges();

            Transaction c8 = new Transaction();
            c8.TypeOfTransaction = 0;
            c8.TransactionDate = new DateTime(2016, 01, 01);
            c8.TransactionDescription = "Initial Deposit";
            c8.TransactionAmount = 7916.40m;
            c8.AccountSender = 1000000007;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c8);
            c8.AppUser = db.Users.SingleOrDefault(s => s.Email == "westj@pioneer.net");
            db.SaveChanges();

            Transaction c10 = new Transaction();
            c10.TypeOfTransaction = 0;
            c10.TransactionDate = new DateTime(2016, 01, 01);
            c10.TransactionDescription = "Initial Deposit";
            c10.TransactionAmount = 69576.83m;
            c10.AccountSender = 1000000009;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c10);
            c10.AppUser = db.Users.SingleOrDefault(s => s.Email == "willsheff@email.com");
            db.SaveChanges();

            Transaction c12 = new Transaction();
            c12.TypeOfTransaction = 0;
            c12.TransactionDate = new DateTime(2016, 01, 01);
            c12.TransactionDescription = "Initial Deposit";
            c12.TransactionAmount = 30279.33m;
            c12.AccountSender = 1000000011;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c12);
            c12.AppUser = db.Users.SingleOrDefault(s => s.Email == "erynrice@aool.com");
            db.SaveChanges();

            Transaction c13 = new Transaction();
            c13.TypeOfTransaction = 0;
            c13.TransactionDate = new DateTime(2016, 01, 01);
            c13.TransactionDescription = "Initial Deposit";
            c13.TransactionAmount = 5000.00m;
            c13.AccountSender = 1000000012;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c13);
            c13.AppUser = db.Users.SingleOrDefault(s => s.Email == "mackcloud@pimpdaddy.com");
            db.SaveChanges();

            Transaction c14 = new Transaction();
            c14.TypeOfTransaction = 0;
            c14.TransactionDate = new DateTime(2016, 01, 01);
            c14.TransactionDescription = "Initial Deposit";
            c14.TransactionAmount = 11958.08m;
            c14.AccountSender = 1000000013;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c14);
            c14.AppUser = db.Users.SingleOrDefault(s => s.Email == "ss34@ggmail.com");
            db.SaveChanges();

            Transaction c15 = new Transaction();
            c15.TypeOfTransaction = 0;
            c15.TransactionDate = new DateTime(2016, 01, 01);
            c15.TransactionDescription = "Initial Deposit";
            c15.TransactionAmount = 72990.47m;
            c15.AccountSender = 1000000014;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c15);
            c15.AppUser = db.Users.SingleOrDefault(s => s.Email == "tanner@ggmail.com");
            db.SaveChanges();

            Transaction c16 = new Transaction();
            c16.TypeOfTransaction = 0;
            c16.TransactionDate = new DateTime(2016, 01, 01);
            c16.TransactionDescription = "Initial Deposit";
            c16.TransactionAmount = 7417.20m;
            c16.AccountSender = 1000000015;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c16);
            c16.AppUser = db.Users.SingleOrDefault(s => s.Email == "liz@ggmail.com");
            db.SaveChanges();

            Transaction c17 = new Transaction();
            c17.TypeOfTransaction = 0;
            c17.TransactionDate = new DateTime(2016, 01, 01);
            c17.TransactionDescription = "Initial Deposit";
            c17.TransactionAmount = 5000.00m;
            c17.AccountSender = 1000000016;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c17);
            c17.AppUser = db.Users.SingleOrDefault(s => s.Email == "ra@aoo.com");
            db.SaveChanges();

            Transaction c19 = new Transaction();
            c19.TypeOfTransaction = 0;
            c19.TransactionDate = new DateTime(2016, 01, 01);
            c19.TransactionDescription = "Initial Deposit";
            c19.TransactionAmount = 1642.82m;
            c19.AccountSender = 1000000018;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c19);
            c19.AppUser = db.Users.SingleOrDefault(s => s.Email == "mclarence@aool.com");
            db.SaveChanges();

            Transaction c20 = new Transaction();
            c20.TypeOfTransaction = 0;
            c20.TransactionDate = new DateTime(2016, 01, 01);
            c20.TransactionDescription = "Initial Deposit";
            c20.TransactionAmount = 84421.45m;
            c20.AccountSender = 1000000019;
            db.Transactions.AddOrUpdate(c => c.TransactionID, c20);
            c20.AppUser = db.Users.SingleOrDefault(s => s.Email == "ss34@ggmail.com");
            db.SaveChanges();
        }

        public static void SeedStocks(AppDbContext db)
		{
			StockList  c1 = new StockList();
			c1.Ticker = "GOOG";
			c1.StockType = LonghornBank.Models.Type.Ordinary;
			c1.Name = "Alphabet Inc.";
			c1.TransactionFee = 25m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c1);
			db.SaveChanges();

			StockList  c2 = new StockList();
			c2.Ticker = "AAPL";
			c2.StockType = LonghornBank.Models.Type.Ordinary;
			c2.Name = "Apple Inc.";
			c2.TransactionFee = 40m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c2);
			db.SaveChanges();

			StockList  c3 = new StockList();
			c3.Ticker = "AMZN";
			c3.StockType = LonghornBank.Models.Type.Ordinary;
			c3.Name = "Amazon.com Inc.";
			c3.TransactionFee = 15m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c3);
			db.SaveChanges();

			StockList  c4 = new StockList();
			c4.Ticker = "LUV";
			c4.StockType = LonghornBank.Models.Type.Ordinary;
			c4.Name = "Southwest Airlines";
			c4.TransactionFee = 35m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c4);
			db.SaveChanges();

			StockList  c5 = new StockList();
			c5.Ticker = "TXN";
			c5.StockType = LonghornBank.Models.Type.Ordinary;
			c5.Name = "Texas Instruments";
			c5.TransactionFee = 15m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c5);
			db.SaveChanges();

			StockList  c6 = new StockList();
			c6.Ticker = "HSY";
			c6.StockType = LonghornBank.Models.Type.Ordinary;
			c6.Name = "The Hershey Company";
			c6.TransactionFee = 25m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c6);
			db.SaveChanges();

			StockList  c7 = new StockList();
			c7.Ticker = "V";
			c7.StockType = LonghornBank.Models.Type.Ordinary;
			c7.Name = "Visa Inc.";
			c7.TransactionFee = 10m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c7);
			db.SaveChanges();

			StockList  c8 = new StockList();
			c8.Ticker = "NKE";
			c8.StockType = LonghornBank.Models.Type.Ordinary;
			c8.Name = "Nike";
			c8.TransactionFee = 30m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c8);
			db.SaveChanges();

			StockList  c9 = new StockList();
			c9.Ticker = "VWO";
			c9.StockType = LonghornBank.Models.Type.ETF;
			c9.Name = "Vanguard Emerging Markets ETF";
			c9.TransactionFee = 20m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c9);
			db.SaveChanges();

			StockList  c10 = new StockList();
			c10.Ticker = "CORN";
			c10.StockType = LonghornBank.Models.Type.Futures;
			c10.Name = "Corn";
			c10.TransactionFee = 10m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c10);
			db.SaveChanges();

			StockList  c12 = new StockList();
			c12.Ticker = "F";
			c12.StockType = LonghornBank.Models.Type.Ordinary;
			c12.Name = "Ford Motor Company";
			c12.TransactionFee = 10m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c12);
			db.SaveChanges();

			StockList  c13 = new StockList();
			c13.Ticker = "BAC";
			c13.StockType = LonghornBank.Models.Type.Ordinary;
			c13.Name = "Bank of America Corporation";
			c13.TransactionFee = 10m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c13);
			db.SaveChanges();

			StockList  c14 = new StockList();
			c14.Ticker = "VNQ";
			c14.StockType = LonghornBank.Models.Type.ETF;
			c14.Name = "Vanguard REIT ETF";
			c14.TransactionFee = 15m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c14);
			db.SaveChanges();

			StockList  c16 = new StockList();
			c16.Ticker = "KMX";
			c16.StockType = LonghornBank.Models.Type.Ordinary;
			c16.Name = "CarMax, Inc.";
			c16.TransactionFee = 15m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c16);
			db.SaveChanges();

			StockList  c17 = new StockList();
			c17.Ticker = "DIA";
			c17.StockType = LonghornBank.Models.Type.Index;
			c17.Name = "Dow Jones Industrial Average Index Fund";
			c17.TransactionFee = 25m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c17);
			db.SaveChanges();

			StockList  c18 = new StockList();
			c18.Ticker = "SPY";
			c18.StockType = LonghornBank.Models.Type.Index;
			c18.Name = "S&P 500 Index Fund";
			c18.TransactionFee = 25m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c18);
			db.SaveChanges();

			StockList  c19 = new StockList();
			c19.Ticker = "BEN";
			c19.StockType = LonghornBank.Models.Type.Ordinary;
			c19.Name = "Franklin Resources, Inc.";
			c19.TransactionFee = 25m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c19);
			db.SaveChanges();

			StockList  c20 = new StockList();
			c20.Ticker = "PGSCX";
			c20.StockType = LonghornBank.Models.Type.Mutual;
			c20.Name = "Pacific Advisors Small Cap Value Fund";
			c20.TransactionFee = 15m;
			db.StockLists.AddOrUpdate(c => c.Ticker, c20);
			db.SaveChanges();

        }

        public static void SeedPayees(AppDbContext db)
		{
			Payee  c1 = new Payee();
			c1.Name = "City of Austin Water";
			c1.PayeeType = PayeeType.Utilities;
			c1.StreetAddress = "113 South Congress Ave.";
			c1.City = "Austin";
			c1.State = StateAbbr.TX;
			c1.ZIP = "78710";
			c1.PhoneNumber = "5126645558";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c1);
			db.SaveChanges();

			Payee  c2 = new Payee();
			c2.Name = "Reliant Energy";
			c2.PayeeType = PayeeType.Utilities;
			c2.StreetAddress = "3500 E. Interstate 10";
			c2.City = "Houston";
			c2.State = StateAbbr.TX;
			c2.ZIP = "77099";
			c2.PhoneNumber = "7135546697";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c2);
			db.SaveChanges();

			Payee  c3 = new Payee();
			c3.Name = "Lee Properties";
			c3.PayeeType = PayeeType.Rent;
			c3.StreetAddress = "2500 Salado";
			c3.City = "Austin";
			c3.State = StateAbbr.TX;
			c3.ZIP = "78705";
			c3.PhoneNumber = "5124453312";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c3);
			db.SaveChanges();

			Payee  c4 = new Payee();
			c4.Name = "Capital One";
			c4.PayeeType = PayeeType.CreditCard;
			c4.StreetAddress = "1299 Fargo Blvd.";
			c4.City = "Cheyenne";
			c4.State = StateAbbr.WY;
			c4.ZIP = "82001";
			c4.PhoneNumber = "5302215542";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c4);
			db.SaveChanges();

			Payee  c5 = new Payee();
			c5.Name = "Vanguard Title";
			c5.PayeeType = PayeeType.Mortgage;
			c5.StreetAddress = "10976 Interstate 35 South";
			c5.City = "Austin";
			c5.State = StateAbbr.TX;
			c5.ZIP = "78745";
			c5.PhoneNumber = "5128654951";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c5);
			db.SaveChanges();

			Payee  c6 = new Payee();
			c6.Name = "Lawn Care of Texas";
			c6.PayeeType = PayeeType.Other;
			c6.StreetAddress = "4473 W. 3rd Street";
			c6.City = "Austin";
			c6.State = StateAbbr.TX;
			c6.ZIP = "78712";
			c6.PhoneNumber = "5123365247";
			db.Payees.AddOrUpdate(c => c.PhoneNumber, c6);
			db.SaveChanges();

        }

	}
}