using Project.DataAccess;
using Project.DataModels;
using Project.Logic;
using System;
using System.Collections.Generic;

namespace Project.Logic
{
    public class Tablelogic
    {
        public int ReturnTableSize(ReservationModel reservation) 
        {     
            if (reservation.TableId == 1 || reservation.TableId == 2 || reservation.TableId == 3 || reservation.TableId == 4)
            { 
                return 2;
            } 
            if (reservation.TableId == 5 || reservation.TableId == 6 || reservation.TableId == 7 || reservation.TableId == 8 || reservation.TableId == 9 || reservation.TableId == 10)
            {
                return 4; 
            } 
            if (reservation.TableId == 11 || reservation.TableId == 12 || reservation.TableId == 13 || reservation.TableId == 14) 
            {
                return 6; 
            } 
            return 0; 
        }
    }
}