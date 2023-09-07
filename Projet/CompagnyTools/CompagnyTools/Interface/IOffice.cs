using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface IOffice
    {
        public List<OfficeModel> OfficeData();
        public List<OfficeModel> UpdateOfficeData(List<OfficeModel> model);
        public DataOffice DuplicateDesk(DataOffice model);
        public List<OfficeModel> CreateAMap(MapCreationModel model);
        public DataOffice CreateDeskSeparator(string designationName);
        public int DeleteDesk(int Id);

        public void CreateReservation(OfficeModel model);
    }
}
