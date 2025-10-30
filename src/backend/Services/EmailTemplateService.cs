using System;
using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services;

public class EmailTemplateService
{
    private readonly IConfiguration _configuration;

    public EmailTemplateService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetEmailHeader(string title)
    {
        var logoUrl = $"{_configuration["AppSettings:FrontendUrl"]}/logo.png";
        
        return $@"
            <!DOCTYPE html>
            <html lang='pt-BR'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                <title>{title}</title>
                <style>
                    * {{
                        margin: 0;
                        padding: 0;
                        box-sizing: border-box;
                    }}
                    
                    body {{
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
                        line-height: 1.6;
                        color: #2c3e50;
                        background: linear-gradient(135deg, #f5f7fa 0%, #e8ecf1 100%);
                        padding: 40px 20px;
                        margin: 0;
                    }}
                    
                    .email-wrapper {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        border-radius: 16px;
                        overflow: hidden;
                        box-shadow: 0 10px 40px rgba(0, 0, 0, 0.08);
                    }}
                    
                    .email-header {{
                        background: linear-gradient(135deg, #ff8f00 0%, #ff6f00 100%);
                        padding: 50px 30px 40px;
                        text-align: center;
                        position: relative;
                    }}
                    
                    .email-header::after {{
                        content: '';
                        position: absolute;
                        bottom: -20px;
                        left: 0;
                        right: 0;
                        height: 40px;
                        background: #ffffff;
                        border-radius: 50% 50% 0 0;
                    }}
                    
                    .logo {{
                        width: 100px;
                        height: auto;
                        margin-bottom: 20px;
                        filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.15));
                    }}
                    
                    .email-header h1 {{
                        color: #ffffff;
                        font-size: 26px;
                        font-weight: 700;
                        margin: 0;
                        text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        letter-spacing: -0.5px;
                    }}
                    
                    .email-body {{
                        padding: 40px 35px;
                        background-color: #ffffff;
                    }}
                    
                    .email-body p {{
                        margin-bottom: 18px;
                        color: #555;
                        font-size: 15px;
                        line-height: 1.7;
                    }}
                    
                    .email-body strong {{
                        color: #2c3e50;
                        font-weight: 600;
                    }}
                    
                    .greeting {{
                        font-size: 20px;
                        color: #2c3e50;
                        margin-bottom: 24px;
                        font-weight: 600;
                    }}
                    
                    .cta-button {{
                        display: inline-block;
                        padding: 16px 40px;
                        background: linear-gradient(135deg, #ff8f00 0%, #ff6f00 100%);
                        color: #ffffff !important;
                        text-decoration: none;
                        border-radius: 50px;
                        font-weight: 600;
                        font-size: 16px;
                        margin: 28px 0;
                        transition: all 0.3s ease;
                        box-shadow: 0 8px 20px rgba(255, 143, 0, 0.35);
                        border: none;
                    }}
                    
                    .cta-button:hover {{
                        transform: translateY(-2px);
                        box-shadow: 0 12px 28px rgba(255, 143, 0, 0.45);
                    }}
                    
                    .cta-container {{
                        text-align: center;
                        margin: 35px 0;
                    }}
                    
                    .info-box {{
                        background: linear-gradient(135deg, #fff8e1 0%, #ffecb3 100%);
                        border-left: 4px solid #ff8f00;
                        padding: 18px 22px;
                        border-radius: 10px;
                        margin: 28px 0;
                        box-shadow: 0 2px 8px rgba(255, 143, 0, 0.1);
                    }}
                    
                    .info-box p {{
                        margin: 0;
                        color: #6d4c00;
                        font-size: 14px;
                        line-height: 1.6;
                    }}
                    
                    .info-box strong {{
                        color: #6d4c00;
                    }}
                    
                    .ticket-box {{
                        background: linear-gradient(135deg, #fafafa 0%, #f5f5f5 100%);
                        border: 1px solid #e0e0e0;
                        border-left: 5px solid #ff8f00;
                        padding: 24px;
                        border-radius: 12px;
                        margin: 28px 0;
                        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                    }}
                    
                    .ticket-box h3 {{
                        color: #2c3e50;
                        font-size: 19px;
                        margin-bottom: 16px;
                        font-weight: 700;
                        display: flex;
                        align-items: center;
                        gap: 8px;
                    }}
                    
                    .ticket-box .ticket-detail {{
                        margin: 12px 0;
                        color: #555;
                        font-size: 15px;
                        display: flex;
                        align-items: flex-start;
                        gap: 10px;
                    }}
                    
                    .ticket-box .ticket-label {{
                        color: #777;
                        font-weight: 600;
                        min-width: 90px;
                        flex-shrink: 0;
                    }}
                    
                    .message-box {{
                        background-color: #ffffff;
                        border: 2px solid #e8ecf1;
                        padding: 20px 24px;
                        border-radius: 12px;
                        margin: 22px 0;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.04);
                    }}
                    
                    .message-box p {{
                        margin: 0;
                        color: #444;
                        white-space: pre-wrap;
                        word-wrap: break-word;
                        line-height: 1.7;
                    }}
                    
                    .link-box {{
                        background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%);
                        padding: 14px 18px;
                        border-radius: 10px;
                        margin: 22px 0;
                        word-break: break-all;
                        border: 1px solid #90caf9;
                    }}
                    
                    .link-box a {{
                        color: #1976d2;
                        text-decoration: none;
                        font-size: 13px;
                        font-weight: 500;
                    }}
                    
                    .divider {{
                        height: 2px;
                        background: linear-gradient(to right, transparent, #e0e0e0 20%, #e0e0e0 80%, transparent);
                        margin: 35px 0;
                    }}
                    
                    .email-footer {{
                        background: linear-gradient(135deg, #f5f5f5 0%, #eeeeee 100%);
                        padding: 35px 30px;
                        text-align: center;
                        border-top: 3px solid #ff8f00;
                    }}
                    
                    .email-footer p {{
                        margin: 10px 0;
                        color: #777;
                        font-size: 13px;
                    }}
                    
                    .email-footer .copyright {{
                        font-weight: 700;
                        color: #2c3e50;
                        font-size: 14px;
                    }}
                    
                    .social-links {{
                        margin: 22px 0 12px;
                    }}
                    
                    .social-links a {{
                        display: inline-block;
                        margin: 0 10px;
                        color: #ff8f00;
                        text-decoration: none;
                        font-size: 14px;
                        font-weight: 600;
                        transition: color 0.3s ease;
                    }}
                    
                    .social-links a:hover {{
                        color: #ff6f00;
                    }}
                    
                    ul {{
                        list-style: none;
                        padding-left: 0;
                    }}
                    
                    ul li {{
                        padding-left: 28px;
                        position: relative;
                        margin: 10px 0;
                    }}
                    
                    ul li::before {{
                        content: '✓';
                        position: absolute;
                        left: 0;
                        color: #ff8f00;
                        font-weight: 700;
                        font-size: 16px;
                    }}
                    
                    @media only screen and (max-width: 600px) {{
                        body {{
                            padding: 20px 10px;
                        }}
                        
                        .email-wrapper {{
                            border-radius: 12px;
                        }}
                        
                        .email-header {{
                            padding: 40px 20px 30px;
                        }}
                        
                        .email-header h1 {{
                            font-size: 22px;
                        }}
                        
                        .email-body {{
                            padding: 30px 20px;
                        }}
                        
                        .logo {{
                            width: 80px;
                        }}
                        
                        .cta-button {{
                            display: block;
                            text-align: center;
                            padding: 14px 30px;
                        }}
                        
                        .ticket-box .ticket-detail {{
                            flex-direction: column;
                            gap: 4px;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class='email-wrapper'>
                    <header class='email-header'>
                        <img src='{logoUrl}' alt='Caju Ajuda Logo' class='logo'>
                        <h1>{title}</h1>
                    </header>
                    <main class='email-body'>
        ";
    }

    private string GetEmailFooter()
    {
        return @"
                    </main>
                    <footer class='email-footer'>
                        <p class='copyright'>© 2025 <strong>Caju Ajuda</strong> - Seu suporte inteligente</p>
                        <div class='social-links'>
                            <a href='#'>Suporte</a> ·
                            <a href='#'>Base de Conhecimento</a> ·
                            <a href='#'>Política de Privacidade</a>
                        </div>
                        <p>Este é um e-mail automático. Por favor, não responda.</p>
                        <p>Em caso de dúvidas, abra um chamado em nosso sistema.</p>
                    </footer>
                </div>
            </body>
            </html>
        ";
    }

    public string GetVerificationEmailBody(string nome, string token)
    {
        var verificationUrl = $"{_configuration["AppSettings:FrontendUrl"]}/verificar-email?token={Uri.EscapeDataString(token)}";

        var body = GetEmailHeader("Confirme seu cadastro");
        
        body += $@"
                        <p class='greeting'>Olá, <strong>{nome}</strong>! 👋</p>
                        
                        <p>Seja muito bem-vindo(a) ao <strong>Caju Ajuda</strong>! 🎉</p>
                        
                        <p>Estamos muito felizes em tê-lo(a) conosco. Para começar a usar todas as funcionalidades da nossa plataforma, precisamos verificar seu endereço de e-mail.</p>
                        
                        <div class='info-box'>
                            <p><strong>🔐 Segurança em primeiro lugar!</strong></p>
                            <p>A verificação garante a segurança da sua conta e permite que você aproveite ao máximo nosso sistema de chamados.</p>
                        </div>
                        
                        <div class='cta-container'>
                            <a href='{verificationUrl}' class='cta-button'>✓ Verificar meu e-mail</a>
                        </div>
                        
                        <p>Ou copie e cole o link abaixo no seu navegador:</p>
                        <div class='link-box'>
                            <a href='{verificationUrl}'>{verificationUrl}</a>
                        </div>
                        
                        <div class='divider'></div>
                        
                        <p><strong>O que você pode fazer após a verificação:</strong></p>
                        <ul>
                            <li>Abrir e acompanhar chamados de suporte</li>
                            <li>Receber notificações em tempo real</li>
                            <li>Consultar nossa base de conhecimento</li>
                            <li>Avaliar o atendimento recebido</li>
                        </ul>
                        
                        <p style='margin-top: 28px;'><strong>Precisa de ajuda?</strong> Nossa equipe está pronta para auxiliá-lo!</p>
        ";
        
        body += GetEmailFooter();
        return body;
    }

    public string GetNovaRespostaEmailBody(string nomeCliente, string numeroChamado, string assuntoChamado, string mensagemResposta, string nomeTecnico)
    {
        var chamadoUrl = $"{_configuration["AppSettings:FrontendUrl"]}/chamado/{numeroChamado}";

        var body = GetEmailHeader("Nova resposta no seu chamado");
        
        body += $@"
                        <p class='greeting'>Olá, <strong>{nomeCliente}</strong>! 👋</p>
                        
                        <p>Ótimas notícias! O técnico <strong>{nomeTecnico}</strong> respondeu ao seu chamado. 🎉</p>
                        
                        <div class='ticket-box'>
                            <h3>📋 Chamado #{numeroChamado}</h3>
                            <div class='ticket-detail'>
                                <span class='ticket-label'>Assunto:</span>
                                <span>{assuntoChamado}</span>
                            </div>
                            <div class='ticket-detail'>
                                <span class='ticket-label'>Técnico:</span>
                                <span>{nomeTecnico}</span>
                            </div>
                        </div>
                        
                        <p><strong>💬 Nova mensagem:</strong></p>
                        <div class='message-box'>
                            <p>{mensagemResposta}</p>
                        </div>
                        
                        <div class='cta-container'>
                            <a href='{chamadoUrl}' class='cta-button'>📨 Ver chamado completo</a>
                        </div>
                        
                        <div class='info-box'>
                            <p><strong>💡 Dica:</strong> Responda diretamente pela plataforma para manter todo o histórico organizado e acessível.</p>
                        </div>
                        
                        <p style='margin-top: 28px; text-align: center; font-size: 14px; color: #777;'>
                            Você está recebendo este e-mail porque tem um chamado ativo em nossa plataforma.
                        </p>
        ";
        
        body += GetEmailFooter();
        return body;
    }

    public string GetChamadoAtribuidoEmailBody(string nomeTecnico, string numeroChamado, string assuntoChamado, string descricaoChamado, string nomeCliente)
    {
        var chamadoUrl = $"{_configuration["AppSettings:FrontendUrl"]}/chamado/{numeroChamado}";

        var body = GetEmailHeader("🎯 Novo Chamado Atribuído");
        
        body += $@"
                        <p class='greeting'>Olá, <strong>{nomeTecnico}</strong>! 👋</p>
                        
                        <p>Um novo chamado foi atribuído a você e está aguardando seu atendimento. 🎯</p>
                        
                        <div class='ticket-box'>
                            <h3>📋 Chamado #{numeroChamado}</h3>
                            <div class='ticket-detail'>
                                <span class='ticket-label'>Cliente:</span>
                                <span>{nomeCliente}</span>
                            </div>
                            <div class='ticket-detail'>
                                <span class='ticket-label'>Assunto:</span>
                                <span>{assuntoChamado}</span>
                            </div>
                        </div>
                        
                        <p><strong>📝 Descrição do problema:</strong></p>
                        <div class='message-box'>
                            <p>{descricaoChamado}</p>
                        </div>
                        
                        <div class='cta-container'>
                            <a href='{chamadoUrl}' class='cta-button'>🚀 Atender chamado</a>
                        </div>
                        
                        <div class='info-box'>
                            <p><strong>⏱ Atenção:</strong> O cliente está aguardando seu retorno. Tente responder o mais breve possível para garantir a melhor experiência.</p>
                        </div>
                        
                        <div class='divider'></div>
                        
                        <p><strong>✨ Boas práticas de atendimento:</strong></p>
                        <ul>
                            <li>Responda em até 24 horas</li>
                            <li>Seja claro e objetivo na comunicação</li>
                            <li>Mantenha o cliente informado sobre o progresso</li>
                            <li>Documente todas as soluções aplicadas</li>
                        </ul>
                        
                        <p style='margin-top: 28px;'><strong>Conte com o suporte da equipe sempre que precisar!</strong> 💪</p>
        ";
        
        body += GetEmailFooter();
        return body;
    }
}
