using AutoMapper;
using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.AutoMapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<DataOffice, DeskModel>();
            CreateMap<DeskModel, DataOffice>();

            CreateMap<Equipments, EquipmentsModel>();
            CreateMap<EquipmentsModel, Equipments>();
        }
    }
}
