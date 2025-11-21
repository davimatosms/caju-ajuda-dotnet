using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using System;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService;
        private readonly EmailTemplateService _emailTemplateService;

        public UsuarioService(IUsuarioRepository usuarioRepository, IEmailService emailService, EmailTemplateService emailTemplateService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<Usuario?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !user.Enabled || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, user.Senha))
            {
                Console.WriteLine($"[AUTH] Falha na autenticação para: {loginDto.Email}");
                return null;
            }

            Console.WriteLine($"[AUTH] Login bem-sucedido: {loginDto.Email}");
            return user;
        }

        public async Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto)
        {
            var existingUser = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
            if (existingUser != null)
            {
                throw new BusinessRuleException("Este e-mail já está cadastrado.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);

            // Gerar token de verificação
            var verificationToken = Guid.NewGuid().ToString();
            Console.WriteLine($"[REGISTER] Token gerado: '{verificationToken}'");
            Console.WriteLine($"[REGISTER] Token length: {verificationToken.Length}");

            var novoUsuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = passwordHash,
                Role = Role.CLIENTE,
                Enabled = false, // Conta desabilitada até verificar email
                VerificationToken = verificationToken
            };

            await _usuarioRepository.AddAsync(novoUsuario);

            // Enviar email de verificação
            Console.WriteLine($"[REGISTER] Enviando email com token: '{verificationToken}'");
            await SendVerificationEmailAsync(novoUsuario.Email, novoUsuario.Nome, verificationToken);

            return novoUsuario;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            Console.WriteLine($"[VERIFY_SERVICE] Buscando usuário com token: '{token}'");
            var user = await _usuarioRepository.GetByVerificationTokenAsync(token);
            
            if (user == null)
            {
                Console.WriteLine($"[VERIFY_SERVICE] ❌ Nenhum usuário encontrado com este token!");
                return false;
            }

            Console.WriteLine($"[VERIFY_SERVICE] ✅ Usuário encontrado: {user.Email}, Enabled: {user.Enabled}");
            user.Enabled = true;
            user.VerificationToken = null;
            await _usuarioRepository.UpdateAsync(user);
            Console.WriteLine($"[VERIFY_SERVICE] ✅ Usuário ativado com sucesso!");
            return true;
        }

        public async Task<PerfilResponseDto> GetPerfilAsync(string userEmail)
        {
            var user = await _usuarioRepository.GetByEmailAsync(userEmail);
            if (user == null) throw new NotFoundException("Usuário não encontrado.");

            return new PerfilResponseDto { Nome = user.Nome, Email = user.Email };
        }

        public async Task UpdatePerfilAsync(string userEmail, PerfilUpdateDto perfilDto)
        {
            var user = await _usuarioRepository.GetByEmailAsync(userEmail);
            if (user == null) throw new NotFoundException("Usuário não encontrado.");

            if (user.Email != perfilDto.Email)
            {
                var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(perfilDto.Email);
                if (existingUserWithEmail != null)
                {
                    throw new BusinessRuleException("O e-mail informado já está em uso por outra conta.");
                }
            }

            user.Nome = perfilDto.Nome;
            user.Email = perfilDto.Email;
            await _usuarioRepository.UpdateAsync(user);
        }

        public async Task UpdateSenhaAsync(string userEmail, SenhaUpdateDto senhaDto)
        {
            var user = await _usuarioRepository.GetByEmailAsync(userEmail);
            if (user == null) throw new NotFoundException("Usuário não encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(senhaDto.SenhaAtual, user.Senha))
            {
                throw new BusinessRuleException("A senha atual está incorreta.");
            }

            user.Senha = BCrypt.Net.BCrypt.HashPassword(senhaDto.NovaSenha);
            await _usuarioRepository.UpdateAsync(user);
        }

        private async Task SendVerificationEmailAsync(string email, string nome, string token)
        {
            var emailBody = _emailTemplateService.GetVerificationEmailBody(nome, token);
            await _emailService.SendEmailAsync(email, "Verificação de E-mail - Caju Ajuda", emailBody);
        }
    }
}
