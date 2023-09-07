using AutoMapper;
using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.AutoMapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<DataOffice, OfficeModel>();
            CreateMap<OfficeModel, DataOffice>();

            CreateMap<Equipments, EquipmentsModel>();
            CreateMap<EquipmentsModel, Equipments>();
        }
    }
}
