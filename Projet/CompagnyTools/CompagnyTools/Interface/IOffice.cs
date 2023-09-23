using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface IOffice
    {
        public List<DataOffice> OfficeData();
        public List<OfficeModel> UpdateOfficeData(List<OfficeModel> model);
        public DataOffice DuplicateDesk(DataOffice model);
        public List<DataOffice>? CreateAMap(MapCreationModel model);
        public int DeleteDesk(int Id);
        public bool CreateReservation(OfficeModel model);
        public List<ReservationResultModel>? GetReservationResult(int deskId);
    }
}
