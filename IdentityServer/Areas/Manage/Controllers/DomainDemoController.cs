using System.Linq;
using System.Threading.Tasks;
using IdentityServer.HttpHandlerBase;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class LocalizationController : BaseController
    {
        private readonly LocalizationModelContext _context;
        private readonly IStringExtendedLocalizerFactory _stringExtendedLocalizerFactory;
        public LocalizationController(LocalizationModelContext context, IStringExtendedLocalizerFactory stringExtendedLocalizerFactory)
        {
            _context = context;
            this._stringExtendedLocalizerFactory = stringExtendedLocalizerFactory;
        }

        // GET: DomainDemo
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            var identityDbContext = _context.LocalizationRecords.OrderBy(d=>d.Id);
            var model = await identityDbContext.ToPagedListAsync(pageIndex, pageSize);

            if (this.IsAjaxRequest())
            {
                return PartialView("_IndexTBody", model);
            }
            else
            {
                return View(model);
            }
        }

        // GET: DomainDemo/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FirstOrDefaultAsync(m => m.Id == id);
            if (record == null)
            {
                return NotFoundView();
            }

            return View(record);
        }

        // GET: DomainDemo/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FindAsync(id);
            if (record == null)
            {
                return NotFoundView();
            }

            return View(record);
        }

        // POST: DomainDemo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Text")] LocalizationRecord tRecord)
        {
            if (id != tRecord.Id)
            {
                return NotFoundView();
            }

            var record = await _context.LocalizationRecords.FindAsync(id);
            record.Text = tRecord.Text;
            TryValidateModel(record);

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DomainExists(record.Id))
                    {
                        return NotFoundView();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(record);
        }

        public IActionResult ResetCache()
        {
            _stringExtendedLocalizerFactory.ResetCache();

            return Json(true);
        }

        private bool DomainExists(long id)
        {
            return _context.LocalizationRecords.Any(e => e.Id == id);
        }
    }
}
