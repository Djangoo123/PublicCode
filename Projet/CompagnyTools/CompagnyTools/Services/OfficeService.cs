﻿using CompagnyTools.Context;
using CompagnyTools.Entities;
using CompagnyTools.Interface;
using CompagnyTools.Models;

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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DeskModel> OfficeData()
        {
            try
            {
                // Init
                List<DeskModel> itemsOffice = new();

                // Get our office data
                List<DataOffice> deskOffice = _context.DataOffices.ToList();

                foreach (DataOffice desk in deskOffice)
                {
                    DeskModel item = new()
                    {
                        Id = desk.DeskId,
                        X = desk.X,
                        Y = desk.Y,
                        Chairdirection = desk.Chairdirection,
                        Equipments = new()
                    };

                    // TODO : refacto this, problem on link between tables
                    List<Equipment> equipment = _context.Equipments.Where(x => x.DeskId == desk.DeskId).ToList();

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
        /// <param name="model">Desk list</param>
        /// <returns></returns>
        public List<DeskModel> UpdateOfficeData(List<DeskModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    DataOffice desk = new()
                    {
                        DeskId = item.Id,
                        X = item.X,
                        Y = item.Y,
                        Chairdirection = item.Chairdirection,
                    };

                    _context.DataOffices.Update(desk);
                    _context.SaveChanges();
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
