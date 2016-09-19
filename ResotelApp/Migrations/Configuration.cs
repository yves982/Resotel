namespace ResotelApp.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
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

                Discount[] discounts = (from el in doc.Descendants("Room")
                                        select el.Descendants("Discount"))
                                        .SelectMany(discnts => discnts)
                                        .Select(discountFromXElement).ToArray();

                IDictionary<string, Option> uniqueOptions = new Dictionary<string, Option>();
                IDictionary<string, Room> uniqueRooms = new Dictionary<string, Room>();
                IDictionary<string, Pack> uniquePacks = new Dictionary<string, Pack>();
                IDictionary<string, Discount> uniqueDiscounts = new Dictionary<string, Discount>();


                foreach (Option opt in options)
                {
                    if (!uniqueOptions.ContainsKey(opt.Id.ToString()))
                    {
                        uniqueOptions.Add(opt.Id.ToString(), opt);
                    }
                }

                foreach(Discount discount in discounts)
                {
                    if(!uniqueDiscounts.ContainsKey(discount.Id.ToString()))
                    {
                        uniqueDiscounts.Add(discount.Id.ToString(), discount);
                    }
                }

                foreach (Room room in rooms)
                {
                    if (!uniqueRooms.ContainsKey(room.Id.ToString()))
                    {
                        List<string> optionsIds = (from el in doc.Descendants("Room")
                                                   where el.Element("Id").Value == room.Id.ToString()
                                                   select el).First().Descendants("Option").Select(e => e.Element("Id").Value)
                                                   .ToList();

                        

                        room.Options.AddRange(optionsIds.Select(id => uniqueOptions[id]));

                        uniqueRooms.Add(room.Id.ToString(), room);
                    }
                    // add missing packs
                    List<Pack> availablePacks = (from el in doc.Descendants("Room")
                                                 where el.Element("Id").Value == room.Id.ToString()
                                                 select el).First().Descendants("Pack")
                                                         .Select(packFromXElement).ToList();

                    
                    availablePacks
                            .Where(pack => !uniquePacks.ContainsKey(pack.Id.ToString()))
                            .ToList()
                            .ForEach(pack =>
                            {
                                uniquePacks.Add(pack.Id.ToString(), pack);
                                room.AvailablePacks.Add(pack);
                            });
                }


                foreach (Option option in uniqueOptions.Values)
                {
                    List<string> roomIds = uniqueRooms.Values.Where(room => room.Options.Any(opt => opt.Id == option.Id))
                        .Select(room => room.Id.ToString())
                        .ToList();

                    List<string> discountIds = (from el in doc.Descendants("Option")
                                               where el.Element("Id").Value == option.Id.ToString()
                                               select el).First().Descendants("Discount").Select(
                                                    e => e.Element("Id").Value)
                                                   .ToList();

                    option.Rooms.AddRange(roomIds.Select(id => uniqueRooms[id]));
                    option.Discounts.AddRange(discountIds.Select( id => uniqueDiscounts[id] ));
                }

                context.Rooms.Include(room => room.Options).Include(room => room.AvailablePacks).Load();
                context.Options.Include(opt => opt.Rooms).Load();
                context.Rooms.AddOrUpdate(room => room.Id, uniqueRooms.Values.ToArray());
                context.Options.AddOrUpdate(opt => opt.Id, uniqueOptions.Values.ToArray());
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
            Room room = new Room
            {
                Id = int.Parse(e.Element("Id").Value),
                BedKind = (BedKind)Enum.Parse(typeof(BedKind), e.Element("BedKind").Value),
                Capacity = int.Parse(e.Element("Capacity").Value),
                Stage = int.Parse(e.Element("Stage").Value),
                Options = new List<Option>(),
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
                HasChooseableDates = e.Element("HasChooseableDates").Value == "" ? false : bool.Parse(e.Element("HasChooseableDates").Value),
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

        private Pack packFromXElement(XElement e)
        {
            Pack pack = new Pack
            {
                Id = int.Parse(e.Element("Id").Value),
                Price = double.Parse(e.Element("PackPrice").Value),
                Quantity = int.Parse(e.Element("PackQuantity").Value)
            };
            return pack;
        }

        private Discount discountFromXElement(XElement e)
        {
            Discount discount = new Discount
            {
                Id = int.Parse(e.Element("Id").Value),
                ReduceByPercent = double.Parse(e.Element("ReduceByPercent").Value, CultureInfo.CreateSpecificCulture("en-US")),
                Validity = dateRangeFromXElement(e.Element("Validity"))
            };

            return discount;
        }

        private DateRange dateRangeFromXElement(XElement e)
        {
            DateRange dateRange = new DateRange
            {
                Start = DateTime.ParseExact(e.Element("Start").Value, "yyyy-MM-dd", CultureInfo.CurrentCulture),
                End = DateTime.ParseExact(e.Element("End").Value, "yyyy-MM-dd", CultureInfo.CurrentCulture)
            };
            return dateRange;
        }
    }
}
