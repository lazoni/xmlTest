using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using XMLTest.Models;

namespace XMLTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Läs in fil
            string filsokvag = @"d:\jimmy\XMLTest\testfil.txt";
            string filsokvagXML = @"d:\jimmy\XMLTest\testfil.xml";

            List<string> rader = new List<string>();
            rader = System.IO.File.ReadAllLines(filsokvag, Encoding.UTF8).ToList();

            //splitta reder till personer
            List<Person> persons = new List<Person>();
            var editingFamilyMember = false;
            foreach (string rad in rader)
            {
                var delar = rad.Split("|").ToList();
                //Om reden startar med P -- Lägg upp ny person -- Stega till nästa rad
                if (rad.StartsWith("P"))
                {
                    editingFamilyMember = false;
                    persons.Add(person(rad));
                }
                else
                {
                    if (delar.First() == "T")
                    {
                        if (editingFamilyMember)
                        {
                            persons.Last().Family.Last().Phone = phone(rad);
                        }
                        else { 
                            persons.Last().Phone = phone(rad);
                        }
                    }
                    ///Om rad startar med A --> Lägg till adress på personen
                    else if (delar.First() == "A")
                    {
                        if (editingFamilyMember)
                        {
                            persons.Last().Family.Last().Address = address(rad);
                        }
                        else
                        {
                            persons.Last().Address = address(rad);
                        }
                    }
                    ///Om rad startar med F --> Lägg till familjemedlem på personen
                    else if (delar.First() == "F")
                    {
                        editingFamilyMember = true;
                        if (persons.Last().Family == null)
                        {
                            persons.Last().Family = new List<Familymember>();
                        }
                        persons.Last().Family.Add(familymember(rad));

                    }
                    else
                    {
                        return View("Index", "Denna rad startar med felaktigt tecken: " + rad);
                        //throw new Exception("Det är fel på texten i filen");
                    }

                }
            }

            //Skapa XML
            XElement doc =
                new XElement("people", from person in persons select
                    new XElement("person",
                        new XElement("firstname", person.Firstname),
                        new XElement("lastname", person.Lastname),
                        new XElement("address",
                            new XElement("street", person.Address.Street),
                            new XElement("place", person.Address.Place), person.Address.Number != null ?
                            new XElement("number", person.Address.Number) : null
                        ),

                        person.Phone != null ?
                                    new XElement("phone",
                                        new XElement("mobile", person.Phone.Mobile),
                                        new XElement("phone2", person.Phone.Phone2)) : null,


                        person.Family != null ?
                        from fm in person.Family select
                            new XElement("family",
                                    new XElement("name", fm.Name),
                                    new XElement("born", fm.Born), fm.Address != null ? 
                                    new XElement("address",
                                        new XElement("street", fm.Address.Street),
                                        new XElement("place", fm.Address.Place), fm.Address.Number != null ?
                                        new XElement("number", fm.Address.Number) : null) : null, fm.Phone != null ?
                                    new XElement("phone", 
                                        new XElement("mobile", fm.Phone.Mobile),
                                        new XElement("phone2", fm.Phone.Phone2)) : null
                                ) : null
                            
                    )    
            );
            doc.Save(filsokvagXML);
            return View("Index", "Filen är nu sparad här: " + filsokvagXML);
        }

        private Familymember familymember(string rad)
        {
            var delar = SplittaRaden(rad);
            Familymember familymember = new Familymember();
            familymember.Name = delar[1];
            familymember.Born = delar[2];
            return familymember;
        }

        private Address address(string rad)
        {
            var delar = SplittaRaden(rad);
            Address address = new Address();
            address.Street = delar[1];
            address.Place = delar[2];
            if (delar.Count > 3)
                address.Number = delar[3];
            return address;
        }

        private Phone phone(string rad)
        {
            var delar = SplittaRaden(rad);
            Phone phone = new Phone();
            phone.Mobile = delar[1];
            phone.Phone2 = delar[2];
            return phone;
        }

        private Person person(string rad)
        {
            var delar = SplittaRaden(rad);
            Person person = new Person();
            person.Firstname = delar[1];
            person.Lastname = delar[2];
            return person;
        }

        private List<string> SplittaRaden(string rad)
        {
            return rad.Split("|").ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}