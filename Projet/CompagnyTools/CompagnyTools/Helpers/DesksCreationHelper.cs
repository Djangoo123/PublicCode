using DAL.Context;
using DAL.Entities;

namespace CompagnyTools.Helpers
{
    public class DesksCreationHelper
    {
        public List<DataOffice> CreateDesks(int numberOfDesk, int numberOfLine, string typeDesk, int maxIdInDB, string location)
        {
            try
            {
                List<DataOffice>? deskToCreate = new();

                for (int i = 0; i < numberOfLine; i++)
                {
                    int IdInProgress = maxIdInDB != 0 ? maxIdInDB + 1 : 1;

                    for (int x = 0; x < numberOfDesk; x++)
                    {
                        DataOffice dataOffice = new()
                        {
                            Id = IdInProgress + 1,
                            Chairdirection = "south",
                            X = x,
                            Y = i,
                            Location = location,
                            Equipments = CreationEquipmentByType(IdInProgress + 1, typeDesk)
                        };

                        deskToCreate.Add(dataOffice);
                    }
                }
                return deskToCreate;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static List<Equipments> CreationEquipmentByType(int deskId, string typeDesk)
        {
            try
            {
                List<Equipments> equipment = new();

                switch (typeDesk)
                {
                    case "Full":
                        equipment = new List<Equipments>()
                        {
                                new Equipments { DeskId = deskId, Type = "desk", Specification = "Simple desk" },
                                new Equipments { DeskId = deskId, Type = "cpu", Specification = "Dual core 2.4 GHz, 16 GB RAM, 256 GB HD" },
                                new Equipments { DeskId = deskId, Type = "monitor", Specification = "HP V197 18.5-inch" },
                                new Equipments { DeskId = deskId, Type = "keyboard", Specification = "HP Ultrathin Wireless Keyboard" },
                                new Equipments { DeskId = deskId, Type = "phone", Specification = "Cisco Phone IP 7960G/7940G" },
                                new Equipments { DeskId = deskId, Type = "chair", Specification = "817L Kare Ergonomic Office Chair" },
                                new Equipments { DeskId = deskId, Type = "mouse", Specification = "HP USB 2 Button Optical Mouse" },
                                new Equipments { DeskId = deskId, Type = "drawer", Specification = "Simple drawer" },
                        };
                        break;
                    case "Laptop":
                        equipment = new List<Equipments>()
                        {
                                new Equipments { DeskId = deskId, Type = "desk", Specification = "Simple desk" },
                                new Equipments { DeskId = deskId, Type = "laptop", Specification = "Laptop Dell Inspiron 15 5000" },
                                new Equipments { DeskId = deskId, Type = "phone", Specification = "Cisco Phone IP 7960G/7940G" },
                                new Equipments { DeskId = deskId, Type = "chair", Specification = "817L Kare Ergonomic Office Chair" },
                                new Equipments { DeskId = deskId, Type = "mouse", Specification = "HP USB 2 Button Optical Mouse" },
                                new Equipments { DeskId = deskId, Type = "drawer", Specification = "Simple drawer" },
                        };
                        break;
                }

                return equipment;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }


}
