using CompagnyTools.Context;
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
                    DeskModel item = new();

                    item.Id = desk.DeskId;
                    item.X = desk.X;
                    item.Y = desk.Y;
                    item.Chairdirection = desk.Chairdirection;
                    item.Equipments = new();

                    // TODO : refacto this, problem on link between tables
                    List<Equipment> equipment = _context.Equipments.Where(x => x.DeskId == desk.DeskId).ToList();

                    foreach (var props in equipment)
                    {
                        EquipmentsModel deskProps = new();
                        deskProps.type = props.Type;
                        deskProps.specification = props.Specification;
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
    }
}
