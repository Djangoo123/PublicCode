using DAL.Entities;

namespace CompagnyTools.Helpers
{
    public class DesksCreationHelper
    {
        public List<DataOffice> CreateDesks(int lineX, int lineY, int typeDesk)
        {
			try
			{
				List<DataOffice> deskToCreate = new();
                int IdInProgress = 0; ;

                // begin by creatin on the axe X
				for (int i = 0; i < lineX; i++) {

                    DataOffice dataOffice = new()
					{
						Id = i + 1,
						Chairdirection = "south",
						X = i,
						Y = lineY,
						Equipments = CreationEquipmentByType(i + 1, typeDesk)
                    };

                    IdInProgress = i;
                    deskToCreate.Add(dataOffice);
				}

                //  axe Y
                for (int i = 0; i < lineX; i++)
                {
                    DataOffice dataOffice = new()
                    {
                        Id = IdInProgress + 1,
                        Chairdirection = "south",
                        X = lineX,
                        Y = i,
                        Equipments = CreationEquipmentByType(IdInProgress + 1, typeDesk)
                    };

                    deskToCreate.Add(dataOffice);
                }

                return deskToCreate;
            } 
            catch (Exception)
			{
				throw;
			}
        }


        private static List<Equipments> CreationEquipmentByType(int deskId, int typeDesk)
        {
            try
            {
                List<Equipments> equipment = new();

                switch (typeDesk)
                {
                    case 1:
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
                    case 2:
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
