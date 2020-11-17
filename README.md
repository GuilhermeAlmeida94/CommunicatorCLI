# Considerações iniciais
Projeto feito baseado no desafio de admissão do Instituto Atlântico.

# Desafio
A empresa Battle Royalle Solutions está com a demanda para controlar máquinas Windows remotamente usando CLI.
A empresa então decidiu que a melhor solução para eles, seria a criação de uma aplicação cliente que seria executado como um Windows Service, e uma aplicação Web para prover a interface que permitiria executar os comandos em uma ou várias máquinas.

Usando a aplicação Web, deve ser possível escolher uma máquina para interagir com ela executando comandos e recebendo os resultados dele, ou seja, ter um terminal na aplicação web.

# Requisitos

- Deve ser possível executar comandos como “dir, cd” do cmd e comandos do powershell também e receber o retorno deles.
- A aplicação cliente deve se registrar na aplicação web, tornando possível a interação com aquela máquina.
- A aplicação web não deve deixar executar comandos em maquinas que não estão com o serviço cliente em execução(possível motivo: máquina desligada).
- Deve ser possível registrar o log de execução de comandos bem como o retorno deles, para serem visualizados posteriormente.
- Deve ser possível executar um mesmo comando em várias máquinas de uma única vez.
- A aplicação cliente deve enviar os seguintes dados da máquina local no momento do registro:
- Nome da máquina
- IP Local
- Antivirus instalado
- Firewall está ativo
- Versão do Windows
- Versão do .NET Framework instalado
- Tamanho dos HDs (disponível e total)
- A aplicação cliente deve ter um instalador fácil.
- O instalador da aplicação cliente deve permitir a instalação sem interface gráfica(instalação quiet).

# Avaliação

Os seguintes pontos serão considerados para uma boa avaliação do seu projeto.

- Arquitetura da aplicação
- Código limpo
- Respeito as boas práticas de programação como KISS, YAGNI, SOLID e etc
- Utilização de padrões de projeto
- Utilização de um framework SPA para o Front-End
- Automatização de build/deploy
- Uso de testes unitários
- Utilização de tecnologias recentes, bem como a versão mais nova do C# / .NET Core 3.1
- Utilização de recursos de integração com GitHub.

# CommunicatorCLI
## Tecnologias utilizadas
- .NET Core 5.0.0
- Angular 10.0.6

## Antes de instalar o CommunicatorCLI.Worker
Antes de tudo é necesssário definir qual o endereço e porta a qual serão feitas as requisições.
```
private readonly String _address = "localhost";
private readonly String _port = "5000";
```
## Instalando o CommunicatorCLI.Worker
Primeiro gere o instalador executando no terminal
```
dotnet publish -o <diretorio>
```
e então, como adiministrador, execute
```
sc create CLIWorker binPath=<diretorio>\CommunicatorCLI.Worker.exe
```
## Antes de iniciar o CommunicatorCLI.API
Análogo ao CLIWorker, o front-end da aplicação CLIWeb precisa ter o endereço e porta definidas.
```
private address: string = 'localhost';
private port: string = '5000';
```

## Modo de usar
Ao CLIWorker logar, um item aparece na barra lateral esquerda com o ícone verde avisando que a máquina está disponível. Quando ela desloga, a caixa de seleção desaparece e o círculo se torna vermelho.
Para enviar comandos, deve-se clicar na caixa de seleção das máquinas disponíveis e digitar no campo de texto na barra inferior. O botão de enviar ou o enter enviam o comando.