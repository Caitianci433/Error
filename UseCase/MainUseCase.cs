using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UseCase
{
    public class MainRequest : URequest<MainResponse>
    { 
    
    }
    public class MainResponse : UResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public IEnumerable<Student> Students { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    interface IMainUseCase: IUseCaseHandler<MainRequest, MainResponse> { }

    public class MainUseCase : IMainUseCase
    {
        private readonly ITestRepository _testRepository;

        public MainUseCase(ITestRepository testRepository) 
        {
            _testRepository = testRepository;
        }

        public async Task<MainResponse> Handle(MainRequest request, CancellationToken cancellationToken)
        {
            var flag = await _testRepository.Test();

            await Task.Delay(100, cancellationToken);
            var stuList = new List<Student> { new Student { Id = 1, Name = "xiaohu" }, new Student { Id = 2, Name = "xiaomin" } };
            var result = new MainResponse { IsError = false, ErrorMessage = "ssssssss", Students = stuList };
            
            return result;
        }
    }





}
