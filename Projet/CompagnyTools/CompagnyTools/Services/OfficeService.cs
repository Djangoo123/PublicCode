using CompagnyTools.Context;
using CompagnyTools.Entities;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        public DeskModel OfficeData()
        {
            DeskModel itemOffice = new();

            // For a test for now
            Dataoffice? deskOffice = _context.Dataoffices.Where(x => x.Id == 1).FirstOrDefault();

            itemOffice.Id = deskOffice.Id;
            itemOffice.X = deskOffice.X;
            itemOffice.Y = deskOffice.Y;
            itemOffice.Chairdirection = deskOffice.Chairdirection; ;
            itemOffice.Equipments = new();

            // TODO : refacto this, problem on link between tables
            List<Equipment> equipment = _context.Equipments.Where(x => x.DeskId == deskOffice.Id).ToList();

            foreach (var item in equipment)
            {
                EquipmentsModel deskProps = new();
                deskProps.type = item.Type;
                deskProps.specification = item.Specification;
                itemOffice.Equipments.Add(deskProps);
            }

            return itemOffice;
        } 
    }
}
