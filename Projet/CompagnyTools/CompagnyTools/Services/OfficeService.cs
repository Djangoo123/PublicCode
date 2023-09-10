using CompagnyTools.Helpers;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;

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
        public List<DataOffice> OfficeData()
        {
            try
            {

                // Get our office data
                List<DataOffice> deskOffice = _context.DataOffice.ToList();

                foreach (DataOffice desk in deskOffice)
                {
                    // TODO : refacto this, problem on link between tables
                    List<Equipments> equipment = _context.Equipments.Where(x => x.DeskId == desk.Id).ToList();

                    foreach (var item in equipment)
                    {
                        Equipments deskProps = new()
                        {
                            DeskId = item.DeskId,
                            Id = item.Id,
                            Type = item.Type,
                            Specification = item.Specification
                        };
                        desk.Equipments.Add(deskProps);
                    }
                }

                return deskOffice;
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
                        Location = item.Location,
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
        public bool CreateReservation(OfficeModel model)
        {
            try
            {
                if (model.UserName != null && model.DateReservationEnd != null && model.DateReservationStart != null)
                {
                    DateTime? date = DateTime.UtcNow;
                    date = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month));

                    // We block all creation beyond the current month
                    if (model.DateReservationEnd.Value.AddDays(1).ToUniversalTime() > date.Value.ToUniversalTime())
                    {
                        return false;
                    }

                    //Must check if the dates are not already taken
                    Reservations? checkReservation = _context.Reservations.FirstOrDefault(x => x.DeskId == model.Id);

                    if (checkReservation != null && 
                        (checkReservation.DateReservationEnd.ToShortDateString() == model.DateReservationEnd.Value.AddDays(1).ToShortDateString()
                        || 
                        checkReservation.DateReservationStart.ToShortDateString() == model.DateReservationStart.Value.AddDays(1).ToShortDateString()))
                    {
                        return false;
                    }

                    Reservations reservations = new()
                    {
                        Username = model.UserName,
                        DateReservationEnd = model.DateReservationEnd.Value.AddDays(1),
                        DateReservationStart = model.DateReservationStart.Value.AddDays(1),
                        DeskId = model.Id,
                        Location = model.Location,
                    };

                    _context.Reservations.Add(reservations);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deskId"></param>
        /// <returns></returns>
        public List<ReservationResultModel>? GetReservationResult(int deskId)
        {
            try
            {
                // Init
                List<ReservationResultModel> result = new();

                DateTime? date = DateTime.UtcNow;
                date = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month));

                List<Reservations>? data = _context.Reservations.Where(x => x.DeskId == deskId && (x.DateReservationStart.ToUniversalTime() >= DateTime.UtcNow && x.DateReservationEnd.ToUniversalTime() <= date.Value.ToUniversalTime())).ToList();

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        ReservationResultModel model = new()
                        {
                            Username = item.Username,
                            DateReservationEnd = item.DateReservationEnd.AddDays(1).ToShortDateString(),
                            DateReservationStart = item.DateReservationStart.AddDays(1).ToShortDateString(),
                            Location = item.Location,
                        };

                        result.Add(model);
                    }
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
