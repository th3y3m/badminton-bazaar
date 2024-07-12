using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ColorService
    {
        private readonly ColorRepository _colorRepository;

        public ColorService(ColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public Color Add(string colorName)
        {
            var color = new Color
            {
                ColorId = "CL" + GenerateId.GenerateRandomId(5),
                ColorName = colorName
            };

            _colorRepository.Add(color);
            return color;
        }

        public void Update(Color color)
        {
            _colorRepository.Update(color);
        }

        public Color GetById(string id) => _colorRepository.GetById(id);

        public List<Color> GetAll()
        {
            return _colorRepository.GetAll();
        }

        public void Delete(string id)
        {
            _colorRepository.Delete(id);
        }

        public void Delete(Color color)
        {
            _colorRepository.Delete(color.ColorId);
        }

        public PaginatedList<Color> GetPaginatedColors(
            string searchQuery,
            string sortBy,
            int pageIndex,
            int pageSize)
        {
            var source = _colorRepository.GetDbSet().AsNoTracking();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                source = source.Where(p => p.ColorName.ToLower().Contains(searchQuery.ToLower()));
            }

            source = sortBy switch
            {
                "color_asc" => source.OrderBy(p => p.ColorName),
                "color_desc" => source.OrderByDescending(p => p.ColorName),
                _ => source
            };

            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<Color>(items, count, pageIndex, pageSize);
        }
    }
}
