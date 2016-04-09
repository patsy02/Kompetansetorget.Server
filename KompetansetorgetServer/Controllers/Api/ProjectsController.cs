﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using KompetansetorgetServer.Models;
using Microsoft.Ajax.Utilities;


namespace KompetansetorgetServer.Controllers.Api
{
    public class ProjectsController : ApiController
    {
        private KompetansetorgetServerContext db = new KompetansetorgetServerContext();
        
        // [HttpGet, Route("api/projects")]
        // Activates the correct method based on query string parameters.
        // At the moment you can not use a combination of different strings (other then orderBy and sortBy)
        public IQueryable Get(string types = "", [FromUri] string[] studyGroups = null, string courses = "",
            string titles = "", string orderBy = "", string sortBy = "")
        {
            if (!types.IsNullOrWhiteSpace())
            {
                IQueryable<Project> projects = GetProjectsByType(types);
                if (orderBy.Equals("desc") || orderBy.Equals("asc"))
                {
                    return GetProjectsSorted(projects, orderBy, sortBy);
                }
                return GetProjectsSerialized(projects);

            }

            if (studyGroups.Length != 0)
            {
                int i = studyGroups.Length;
                IQueryable<Project> projects = GetProjectsByStudy(studyGroups);
                if (orderBy.Equals("desc") || orderBy.Equals("asc"))
                {
                    return GetProjectsSorted(projects, orderBy, sortBy);
                }
                return GetProjectsSerialized(projects);
            }

            if (!courses.IsNullOrWhiteSpace())
            {
                IQueryable<Project> projects = GetProjectsByCourse(courses);
                if (orderBy.Equals("desc") || orderBy.Equals("asc"))
                {
                    return GetProjectsSorted(projects, orderBy, sortBy);
                }
                return GetProjectsSerialized(projects);
            }

            if (!titles.IsNullOrWhiteSpace())
            {
                IQueryable<Project> projects = GetProjectsByTitle(titles);
                if (orderBy.Equals("desc") || orderBy.Equals("asc"))
                {
                    return GetProjectsSorted(projects, orderBy, sortBy);
                }
                return GetProjectsSerialized(projects);
            }

            if (orderBy.Equals("desc") || orderBy.Equals("asc"))
            {
                return GetProjectsSorted(orderBy, sortBy);
            }

            return GetProjects();
        }

        // GET: api/Projects
        //public IQueryable<Project> GetProjects()
        /// <summary>
        /// This method is called if no query strings are presented
        /// </summary>
        /// <returns></returns>
        private IQueryable GetProjects()
        {
            var projects = from project in db.projects select project;
            return GetProjectsSerialized(projects);
        }

        // GET: api/Projects/5
        // Example: /api/projects/    2c70edff-edbe-4d6d-8e79-10a47f330feb
        [HttpGet, Route("api/v1/projects/{id}")]
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> GetProject(string id)
        {
            Project project = await db.projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            //return Ok(project);
            return Ok(new
            {
                project.uuid,
                project.title,
                project.description,
                project.webpage,
                project.linkedInProfile,            
                project.stepsToApply,
                project.created,
                project.published,
                project.modified,
                project.status,
                project.tutor,
                companies = project.companies.Select(c => new { c.id }),
                contacts = project.contacts.Select(c => new { c.id }),
                courses = project.courses.Select(c => new { c.id }),
                approvedCourses = project.approvedCourses.Select(c => new { c.id }),
                degrees = project.degrees.Select(d => new { d.id }),
                jobTypes = project.jobTypes.Select(jt => new { jt.id }),
                studyGroups = project.studyGroups.Select(st => new { st.id })
            });
        }

        /// <summary>
        /// Lists all projects that contains that spesific Course. 
        /// GET: api/projects?courses=IS-202
        /// GET: api/projects?courses=IS-304
        /// </summary>
        /// <param name="courses">the courses identificator</param>
        /// <returns></returns> 
        private IQueryable<Project> GetProjectsByCourse(string courses = "")
        {
            var projects = from project in db.projects
                       where project.courses.Any(c => c.id.Equals(courses))
                       select project;

            return projects;
        }

        /// <summary>
        /// Lists all projects that contains that spesific StudyGroup. 
        /// GET: api/projects?studyGroups=datateknologi
        /// GET: api/projects?studyGroups=idrettsfag
        /// Also supports combinations:
        /// GET: api/projects/?studygroups=idrettsfag&studygroups=lærerutdanning
        /// </summary>
        /// <param name="studyGroups">the StudyGroup identificator</param>
        /// <returns></returns> 
       // [HttpGet, Route("api/projects")]
        private IQueryable<Project> GetProjectsByStudy(string[] studyGroups = null)
        {
            var projects = from project in db.projects
                       where project.studyGroups.Any(s => studyGroups.Contains(s.id))
                       select project;

            return projects;
        }

        /// <summary>
        /// List all projects that contain that exact title (could be improved upon)
        /// 
        /// GET: api/projects?titles=Morseffekter%på%eggstørrelse%hos%hummer
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private IQueryable<Project> GetProjectsByTitle(string titles)
        {
            // Delimiter % should be reviewed for change.
            titles = titles.Replace("%", " ");
            var projects = from project in db.projects
                       where project.title.Equals(titles)
                       select project;

            return projects;
        }


        /// <summary>
        /// Lists all projects that contains that spesific ProjectType (full time and part time projects). 
        /// GET: api/projects?types=heltid
        /// GET: api/projects?types=deltid
        /// </summary>
        /// <param name="types">the jobTypes identificator</param>
        /// <returns></returns> 
        // [HttpGet, Route("api/projects")]
        private IQueryable<Project> GetProjectsByType(string types = "")
        {
            var projects = from project in db.projects
                       where project.jobTypes.Any(jt => jt.id.Equals(types))
                       select project;

            return projects;
        }


        /// <summary>
        /// Serializes the project object for json.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns> 
        private IQueryable GetProjectsSerialized(IQueryable<Project> projects)
        {
            return projects.Select(p => new
            {
                p.uuid,
                p.title,
                p.description,
                p.webpage,
                p.linkedInProfile,
                p.stepsToApply,
                p.created,
                p.published,
                p.modified,
                p.status,
                p.tutor,
                companies = p.companies.Select(c => new { c.id }),
                courses = p.courses.Select(l => new { l.id }),
                approvedCourses = p.approvedCourses.Select(c => new { c.id }),
                degrees = p.degrees.Select(d => new { d.id }),
                jobTypes = p.jobTypes.Select(jt => new { jt.id }),
                studyGroups = p.studyGroups.Select(st => new { st.id })
            });
        }

        /// <summary>
        /// List projects in a ascending or descending order based on sortBy parameter.
        /// Examples for use:
        /// GET: api/projects/?course=IS-202&orderby=asc&sortby=published
        /// GET: api/projects/?type=deltid&orderby=desc&sortby=expirydate

        /// </summary>
        /// <param name="queryResult">A result of a query in table Projects</param>
        /// <param name="orderBy">asc = ascending 
        ///                       desc = descending</param>
        /// <param name="sortBy">published = the date a project was published
        ///                      expirydate = the last date to apply for the project</param>
        /// <returns></returns>
        private IQueryable GetProjectsSorted(IQueryable<Project> queryResult, string orderBy = "", string sortBy = "")
        {
            var projects = queryResult.Select(p => new
            {
                p.uuid,
                p.title,
                p.description,
                p.webpage,
                p.linkedInProfile,
                p.stepsToApply,
                p.created,
                p.published,
                p.modified,
                p.status,
                p.tutor,
                companies = p.companies.Select(c => new { c.id }),
                courses = p.courses.Select(l => new { l.id }),
                approvedCourses = p.approvedCourses.Select(c => new { c.id }),
                degrees = p.degrees.Select(d => new { d.id }),
                jobTypes = p.jobTypes.Select(jt => new { jt.id }),
                studyGroups = p.studyGroups.Select(st => new { st.id })
            });

            if (orderBy.Equals("desc"))
            {
                switch (sortBy)
                {
                    case "published":
                        projects = projects.OrderByDescending(j => j.published);
                        return projects;

                    default:
                        return GetProjects();
                }
            }

            switch (sortBy)
            {
                case "published":
                    projects = projects.OrderBy(j => j.published);
                    return projects;

                default:
                    return GetProjects();
            }
        }

        /// <summary>
        /// List projects in a ascending or descending order based on sortBy parameter.
        /// GET: api/projects/?orderby=asc&sortby=published
        /// GET: api/projects/?orderby=desc&sortby=expirydate
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        private IQueryable GetProjectsSorted(string orderBy = "", string sortBy = "")
        {

            var queryResult = from project in db.projects select project;

            // Won't work due to incompatible return type.
            //GetProjectsSerialized(projects1)

            var projects = queryResult.Select(p => new
            {
                p.uuid,
                p.title,
                p.description,
                p.webpage,
                p.linkedInProfile,
                p.stepsToApply,
                p.created,
                p.published,
                p.modified,
                p.status,
                p.tutor,
                companies = p.companies.Select(c => new { c.id }),
                courses = p.courses.Select(l => new { l.id }),
                approvedCourses = p.approvedCourses.Select(c => new { c.id }),
                degrees = p.degrees.Select(d => new { d.id }),
                jobTypes = p.jobTypes.Select(jt => new { jt.id }),
                studyGroups = p.studyGroups.Select(st => new { st.id })
            });

            if (orderBy.Equals("desc"))
            {
                switch (sortBy)
                {
                    case "published":
                        projects = projects.OrderByDescending(j => j.published);
                        return projects;
                    default:
                        return GetProjects();
                }
            }

            switch (sortBy)
            {
                case "published":
                    projects = projects.OrderBy(j => j.published);
                    return projects;
                default:
                    return GetProjects();
            }
        }


        // PUT: api/Projects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProject(string id, Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.uuid)
            {
                return BadRequest();
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> PostProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.projects.Add(project);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectExists(project.uuid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = project.uuid }, project);
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> DeleteProject(string id)
        {
            Project project = await db.projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            db.projects.Remove(project);
            await db.SaveChangesAsync();

            return Ok(project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectExists(string id)
        {
            return db.projects.Count(e => e.uuid == id) > 0;
        }
    }
}