using Implementacija.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Implementacija.Services
{
    public class RezervacijaManager : IRezervacijaManager
    {
        
        public async Task<double> calculatePrice(TipMjesta t, int koncertId)
        {
            if (t == TipMjesta.VIP) return 300;
            else if (t == TipMjesta.TRIBINA) return 200;
            else return 100;
        }
        public async Task<IDictionary<string, double>> GeneratePrices (int koncertId)
        {
            IDictionary<string, double> cijene = new Dictionary<string, double>(); 
            foreach (TipMjesta enumValue in Enum.GetValues(typeof(TipMjesta)))
            {
                string enumString = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName();
                cijene[enumString] = await calculatePrice(enumValue, koncertId);
            }
            return cijene;
        }
    }
}
