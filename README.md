# Projeto .NET 8: API e ConsoleApp

Este documento descreve como executar a aplicação, a estrutura de pastas do projeto, as decisões de design e uma descrição simplificada da API Rest.

---

## Como Executar a Aplicação

### API

1. Abra o projeto no Visual Studio ou seu editor de preferência.
2. Defina o projeto `Api` como **Startup Project**.
3. Execute a aplicação via depuração (F5) ou use o comando `dotnet run` no terminal, na pasta do projeto `Api`.

### ConsoleApp

1. Defina o projeto `ConsoleApp` como **Startup Project**.
2. Execute a aplicação via depuração (F5) ou use o comando `dotnet run` no terminal, na pasta do projeto `ConsoleApp`.
3. O ConsoleApp irá criar um arquivo CSV chamado `rotas.csv` no seguinte caminho:
   - `C:/ArquivoEstatico/teste.csv`
4. O arquivo `rotas.csv` terá o seguinte conteúdo:
    ```
    GRU,BRC,10
    BRC,SCL,5
    GRU,CDG,75
    GRU,SCL,20
    GRU,ORL,56
    ORL,CDG,5
    SCL,ORL,20
    ```

---

## Estrutura de Pastas do Projeto

A estrutura do projeto segue o padrão **Domain-Driven Design (DDD)** e está organizada em camadas principais para separar responsabilidades de forma clara:

```text
/ProjetoRaiz
  ├── /Api               # Camada de apresentação (API REST)
  ├── /ConsoleApp        # Aplicação console para manipulação de arquivos CSV
  ├── /Core              # Camada de domínio central, contendo entidades, serviços e interfaces de negócio
  ├── /Domain            # Contém as entidades de domínio
  ├── /Infrastructure    # Integrações externas, como repositórios e persistência de dados (armazenamento de arquivos)
  ├── /UnitTest          # Projeto de testes unitários
```

### Descrição dos Pacotes

- **Api**: Contém a aplicação web que expõe a API REST responsável por fornecer endpoints para manipulação e consulta de rotas.
  
- **ConsoleApp**: Uma aplicação de console que gera o arquivo CSV com as rotas de viagem. 

- **Core**: Contém os serviços e interfaces principais que implementam a lógica de negócio.

- **Domain**: Responsável pelas entidades de domínio que modelam as informações.

- **Infrastructure**: Implementa o repositório de persistência de dados, neste caso utilizando arquivos CSV como meio de armazenamento. A infraestrutura é abstrata da lógica de negócio, permitindo que futuras mudanças de persistência (como bancos de dados) sejam aplicadas sem modificar o Core.

- **UnitTest**: Contém os testes unitários para verificar o comportamento correto dos serviços e lógica de negócio implementados no Core e Domain.

---

## Decisões de Design

1. **Padrão Domain-Driven Design (DDD)**: Foi adotado para garantir que a lógica de negócio seja clara e centralizada. Com o DDD, as entidades e serviços que lidam com o domínio de rotas de viagem são separados da camada de infraestrutura e da API, promovendo coesão e escalabilidade.

2. **Camada de Serviços no Core**: O Core atua como o coração da aplicação, onde os serviços, como o cálculo da rota mais barata, são definidos. Isso facilita a manutenção e evolução da lógica de negócio sem que as camadas de apresentação e infraestrutura sejam afetadas diretamente.

---

## Descrição Simplificada da API Rest

### Contrato da API

A API oferece dois endpoints principais relacionados à gestão de rotas de viagem.

#### 1. `GET /Travel`
Este endpoint busca a rota mais barata entre dois locais especificados (origem e destino).

- **Parâmetros**:
  - `origem`: (String) O local de partida.
  - `destino`: (String) O local de chegada.

- **Respostas**:
  - `200`: Retorna a rota mais barata em formato de string.
  - `204`: Nenhuma rota foi encontrada para os parâmetros fornecidos.
  - `400`: Parâmetros inválidos na requisição.
  - `500`: Erro interno no servidor.

#### 2. `POST /Travel`
Este endpoint permite importar um arquivo CSV contendo rotas de viagem para que a API possa processar esses dados.

- **Corpo da Requisição**:
  - Arquivo CSV enviado via `multipart/form-data`.

- **Respostas**:
  - `200`: Arquivo importado e processado com sucesso.
  - `500`: Ocorreu um erro ao processar o arquivo.
