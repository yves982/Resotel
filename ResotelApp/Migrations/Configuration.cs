namespace ResotelApp.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ResotelApp.Models.Context.ResotelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ResotelApp.Models.Context.Resotel";
        }

        protected override void Seed(ResotelApp.Models.Context.ResotelContext context)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(new Uri(asm.CodeBase).LocalPath), "../../InitialData/Rooms.xml")))
            {
                XDocument doc = XDocument.Load(stream);

                Room[] rooms = (from el in doc.Descendants("Room")
                                select el).Select(roomFromXElment).ToArray();

                Option[] options = (from el in doc.Descendants("Option")
                                    select el).Select(optionFromXElement).ToArray();

                IDictionary<string, Option> uniqueOptions = new Dictionary<string, Option>();
                IDictionary<string, Room> uniqueRooms = new Dictionary<string, Room>();

                foreach(Option opt in options)
                {
                    if(!uniqueOptions.ContainsKey(opt.Id.ToString()))
                    {
                        uniqueOptions.Add(opt.Id.ToString(), opt);
                    }
                }

                foreach(Room room in rooms)
                {
                    if(!uniqueRooms.ContainsKey(room.Id.ToString()))
                    {
                        List<string> optionsIds = (from el in doc.Descendants("Room")
                                                   where el.Element("Id").Value == room.Id.ToString()
                                                   select el).First().Descendants("Option").Select(e => e.Element("Id").Value)
                                                   .ToList();


                        room.Options.AddRange(optionsIds.Select( id=> uniqueOptions[id] ));
                        uniqueRooms.Add(room.Id.ToString(), room);
                    }
                }


                foreach(Option option in uniqueOptions.Values)
                {
                    List<string> roomIds = uniqueRooms.Values.Where(room => room.Options.Any(opt => opt.Id == option.Id))
                        .Select(room => room.Id.ToString())
                        .ToList();
                    
                    option.Rooms.AddRange(roomIds.Select(id => uniqueRooms[id]));
                }

                context.Set<Room>().AddOrUpdate(room => room.Id, uniqueRooms.Values.ToArray());
                context.Set<Option>().AddOrUpdate(opt => opt.Id, uniqueRooms.Values.Select(room => room.Options)
                    .Aggregate((optsRoom1, optsRoom2) =>
                    {
                        List<Option> opts = new List<Option>();
                        opts.AddRange(optsRoom1);
                        opts.AddRange(optsRoom2);
                        return opts;
                    }).ToArray());
                
            }

            using (Stream stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(new Uri(asm.CodeBase).LocalPath), "../../InitialData/Users.xml")))
            {
                XDocument doc = XDocument.Load(stream);

                User[] users = (from el in doc.Descendants("User")
                                select el).Select(userFromXElement).ToArray();

                context.Users.AddOrUpdate(users);
            }

        }

        private Room roomFromXElment(XElement e)
        {
            Room room = new Models.Room
            {
                Id = int.Parse(e.Element("Id").Value),
                BedKind = (BedKind)Enum.Parse(typeof(BedKind), e.Element("BedKind").Value),
                Size = int.Parse(e.Element("Size").Value),
                Stage = int.Parse(e.Element("Stage").Value),
                Options = new List<Models.Option>(),
                IsCleaned = e.Element("IsCleaned").Value == "" ? false : bool.Parse(e.Element("IsCleaned").Value)
            };
            return room;
        }

        private Option optionFromXElement(XElement e)
        {
            Option option = new Option
            {
                Id = int.Parse(e.Element("Id").Value),
                BasePrice = double.Parse(e.Element("BasePrice").Value, new CultureInfo("en-US")),
                Label = e.Element("Label").Value,
                Rooms = new List<Models.Room>()
            };
            return option;
        }

        private User userFromXElement(XElement e)
        {
            User user = new User
            {
                Id = int.Parse(e.Element("Id").Value),
                Login = e.Element("Login").Value,
                Password = e.Element("Password").Value,
                FirstName = e.Element("FirstName").Value,
                LastName = e.Element("LastName").Value,
                Email = e.Element("Email").Value,
                Manager = bool.Parse(e.Element("Manager").Value),
                Rights = (UserRights)Enum.Parse(typeof(UserRights), e.Element("Rights").Value),
                Service = e.Element("Service").Value
            };
            return user;
        }
    }
}
