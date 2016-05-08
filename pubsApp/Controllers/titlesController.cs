using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using pubsApp.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace pubsApp.Controllers
{
    public class titlesController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/titles
        public IQueryable<title> Gettitles(string q = null, string sort = null, bool desc = false,
                                                        int? limit = null, int offset = 0)
        {
            var list = ((IObjectContextAdapter)db).ObjectContext.CreateObjectSet<title>();

            IQueryable<title> items = string.IsNullOrEmpty(sort) ? list.OrderBy(o => o.title1)
                : list.OrderBy(String.Format("it.{0} {1}", sort, desc ? "DESC" : "ASC"));

            if (!string.IsNullOrEmpty(q) && q != "undefined") items = items.Where(t => t.title1.Contains(q));

            if (offset > 0) items = items.Skip(offset);
            if (limit.HasValue) items = items.Take(limit.Value);
            return items;
        }
//            return db.titles;
//      }

        // GET: api/titles/5
        [ResponseType(typeof(title))]
        public IHttpActionResult Gettitle(string id)
        {
            title title = db.titles.Find(id);
            if (title == null)
            {
                return NotFound();
            }

            return Ok(title);
        }

        // PUT: api/titles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttitle(string id, title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != title.title_id)
            {
                return BadRequest();
            }

            db.Entry(title).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!titleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/titles
        [ResponseType(typeof(title))]
        public IHttpActionResult Posttitle(title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.titles.Add(title);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (DbUpdateException)
            {
                if (titleExists(title.title_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = title.title_id }, title);
        }

        // DELETE: api/titles/5
        [ResponseType(typeof(title))]
        public IHttpActionResult Deletetitle(string id)
        {
            title title = db.titles.Find(id);
            if (title == null)
            {
                return NotFound();
            }

            db.titles.Remove(title);
            db.SaveChanges();

            return Ok(title);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool titleExists(string id)
        {
            return db.titles.Count(e => e.title_id == id) > 0;
        }
    }
}