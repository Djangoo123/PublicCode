using CompagnyTools.Helpers;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompagnyTools.Services
{
    public class OfficeService : IOffice
    {
        private readonly Access _context;

        public OfficeService(Access context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all the office data
        /// </summary>
        /// <returns></returns>
        public List<OfficeModel> OfficeData()
        {
            try
            {
                // Init
                List<OfficeModel> itemsOffice = new();

                // Get our office data
                List<DataOffice> deskOffice = _context.DataOffice.ToList();

                foreach (DataOffice desk in deskOffice)
                {
                    OfficeModel item = new()
                    {
                        Id = desk.Id,
                        X = desk.X,
                        Y = desk.Y,
                        Chairdirection = desk.Chairdirection,
                        Equipments = new()
                    };

                    // TODO : refacto this, problem on link between tables
                    List<Equipments> equipment = _context.Equipments.Where(x => x.DeskId == desk.Id).ToList();

                    foreach (var props in equipment)
                    {
                        EquipmentsModel deskProps = new()
                        {
                            DeskId = props.DeskId,
                            Id = props.Id,
                            type = props.Type,
                            specification = props.Specification
                        };
                        item.Equipments.Add(deskProps);
                    }

                    itemsOffice.Add(item);
                }

                return itemsOffice;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Update of offices inside our map
        /// </summary>
        /// <param name = "model" > Desk list</param>
        /// <returns></returns>
        public List<OfficeModel> UpdateOfficeData(List<OfficeModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    DataOffice desk = new()
                    {
                        Id = item.Id,
                        X = item.X,
                        Y = item.Y,
                        Chairdirection = item.Chairdirection,
                    };

                    _context.DataOffice.Update(desk);
                    _context.SaveChanges();
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create a default map with a model parameters
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OfficeModel> CreateAMap(MapCreationModel model)
        {
            try
            {
                // delete all existings records
                _context.Equipments.ExecuteDelete();
                _context.DataOffice.ExecuteDelete();

                DesksCreationHelper desksCreationHelper = new();
                var test = desksCreationHelper.CreateDesks(model.LineX, model.LineY, model.TypeDesk);

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Service duplicating an office. We retrieve the max id, we insert the equipment and then the office.
        /// </summary>
        /// <param name="model">Office data</param>
        /// <returns></returns>
        public DataOffice DuplicateDesk(DataOffice model)
        {
            try
            {
                int id = _context.DataOffice.Max(e => e.Id);

                model.Id = id + 1;

                foreach (var item in model.Equipments)
                {
                    item.DeskId = id;
                    item.Id = 0;
                }

                _context.Equipments.AddRange(model.Equipments);
                _context.DataOffice.Add(model);
                _context.SaveChanges();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create a fake desk to designate a part of the space
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        public DataOffice CreateDeskSeparator(string designationName)
        {
            try
            {
                DataOffice separator = new()
                {
                    Id = _context.DataOffice.Max(e => e.Id) + 1,
                    X = 0,
                    Y = 0,
                    Chairdirection = "south",
                };

                Equipments newEquipment = new()
                {
                    DeskId = separator.Id,
                    Type = "desk",
                    Specification = designationName,
                };

                separator.Equipments.Add(newEquipment);

                _context.Equipments.Add(newEquipment);
                _context.DataOffice.Add(separator);
                _context.SaveChanges();

                return separator;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Service removing an office and all its equipment
        /// </summary>
        /// <param name="Id">Desk Id</param>
        /// <returns></returns>
        public int DeleteDesk(int Id)
        {
            try
            {
                DataOffice? itemToDelete = _context.DataOffice.FirstOrDefault(x => x.Id == Id);
                itemToDelete.Equipments = _context.Equipments.Where(x => x.DeskId == itemToDelete.Id).ToList(); // TODO refacto this, problem link between tables

                if (itemToDelete != null)
                {
                    _context.Equipments.RemoveRange(itemToDelete.Equipments);
                    _context.DataOffice.Remove(itemToDelete);
                    _context.SaveChanges();
                }

                return Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Creation of the reservation then insertion in the database (after checking certain data)
        /// </summary>
        /// <param name="model">Data model</param>
        public void CreateReservation(OfficeModel model)
        {
            try
            {
                if(model.UserName != null && model.DateReservationEnd != null && model.DateReservationStart != null ) 
                {
                    Reservations reservations = new()
                    {
                        Username = model.UserName,
                        DateReservationEnd = model.DateReservationEnd.Value,
                        DateReservationStart = model.DateReservationStart.Value,
                        DeskId = model.Id,
                    };

                    _context.Reservations.Add(reservations);
                    _context.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
