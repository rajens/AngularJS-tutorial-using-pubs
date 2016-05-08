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

namespace pubsApp.Controllers
{
    public class authorsController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/authors
        public IQueryable<author> Getauthors()
        {
            return db.authors;
        }

        // GET: api/authors/5
        [ResponseType(typeof(author))]
        public IHttpActionResult Getauthor(string id)
        {
            author author = db.authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT: api/authors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putauthor(string id, author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != author.au_id)
            {
                return BadRequest();
            }

            db.Entry(author).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!authorExists(id))
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

        // POST: api/authors
        [ResponseType(typeof(author))]
        public IHttpActionResult Postauthor(author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.authors.Add(author);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (authorExists(author.au_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = author.au_id }, author);
        }

        // DELETE: api/authors/5
        [ResponseType(typeof(author))]
        public IHttpActionResult Deleteauthor(string id)
        {
            author author = db.authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }

            db.authors.Remove(author);
            db.SaveChanges();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool authorExists(string id)
        {
            return db.authors.Count(e => e.au_id == id) > 0;
        }
    }
}