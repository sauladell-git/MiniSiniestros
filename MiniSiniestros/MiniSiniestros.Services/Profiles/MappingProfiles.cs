using AutoMapper;
using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Dto.Trabajador;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Services.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Empleador 
            CreateMap<Empleador, EmpleadorDto>().ReverseMap();

            // Prestador 
            CreateMap<Prestador, PrestadorDto>().ReverseMap();

            // Trabajador
            CreateMap<Trabajador, TrabajadorDto>().ReverseMap();

            // SiniestroEstado 
            CreateMap<SiniestroEstado, SiniestroEstadoDto>().ReverseMap();

            // SiniestroEstadoHistorial 
            CreateMap<SiniestroEstadoHistorial, SiniestroEstadoHistorialDto>()
                .ForMember(dest => dest.SiniestroEstadoNombre, opt => opt.MapFrom(src => src.SiniestroEstado != null ? src.SiniestroEstado.Nombre : string.Empty));

            // Siniestro
            CreateMap<Siniestro, SiniestroDto>()
                .ForMember(dest => dest.Prestadores, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialEstados, opt => opt.Ignore());

            CreateMap<CreateSiniestroDto, Siniestro>();

            // NotificacionSRT
            CreateMap<NotificacionSRT, NotificacionSrtDto>().ReverseMap();
        }
    }
}
