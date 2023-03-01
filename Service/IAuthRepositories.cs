using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;

namespace MyBackend_MongoDB_CSharp.Service
{
  public interface IAuthRepositories
  {
    Task<IEnumerable<Auth>> Get_All_Users();
    Task<Auth> GetUserById(string id);
    // login
    Task Login(AuthDTO userDTO);
    // register
    Task Register(Auth user);
    // update
    Task<Auth> UpdateUser(string id, Auth user);
    // delete
    Task DeleteUser(string id);
  }
}