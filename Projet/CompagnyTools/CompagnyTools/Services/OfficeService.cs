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
        public List<DeskModel> OfficeData()
        {
            try
            {
                // Init
                List<DeskModel> itemsOffice = new();

                // Get our office data
                List<DataOffice> deskOffice = _context.DataOffice.ToList();

                foreach (DataOffice desk in deskOffice)
                {
                    DeskModel item = new()
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
        public List<DeskModel> UpdateOfficeData(List<DeskModel> model)
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

        public List<DeskModel> CreateAMap(MapCreationModel model)
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
    }
}
