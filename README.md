# api# Sistema de Loja de Autopeças — Gestão da Qualidade de Software

Projeto prático desenvolvido para a disciplina **Gestão da Qualidade de Software** (Ciência da Computação — UNIFENAS, 2026/2), com foco na implementação de regras de negócio orientadas a domínio, controle de versionamento com **Gitflow** e verificação automatizada por meio de **testes unitários**.

---

## Domínio da Aplicação

O sistema modela o núcleo de operações de uma **Loja de Autopeças e Acessórios Automotivos**, contemplando:

1. **Cadastro de Clientes (`Cliente` / `ClienteService`):**
   * Suporte a Pessoa Física (CPF) e Pessoa Jurídica (CNPJ) com validação matemática dos dígitos verificadores.
   * Validação de formato de e-mail e obrigatoriedade de todos os campos (nome, documento, telefone, e-mail e endereço).
   * Bloqueio de cadastros duplicados para o mesmo CPF/CNPJ.
2. **Catálogo de Produtos (`Produto` / `CategoriaProduto`):**
   * Classificação por categoria (`Motor`, `Suspensao`, `Freios`, `Eletrica`, `Acessorios`, `Fluidos`).
   * Controle de preço unitário (`> 0`), saldo em estoque (`>= 0`), código de barras e número de série vinculado ao modelo/aplicação do veículo compatível.
3. **Registro de Vendas (`Venda` / `VendaService`):**
   * Vinculação obrigatória a um cliente cadastrado, permitindo múltiplas compras recorrentes pelo mesmo cliente.
   * Suporte a múltiplos itens por venda com quantidades individuais.
   * Aplicação de desconto em valor fixo (`R$`), bloqueando descontos negativos ou superiores ao subtotal.
   * Validação prévia de disponibilidade e baixa automática no estoque ao concluir a venda.

---

## Tecnologias e Framework de Testes

* **Linguagem / Plataforma:** C# (.NET 8)
* **Framework de Testes Unitários:** **xUnit** (`2.5.3`) com `Microsoft.NET.Test.Sdk`
* **Arquitetura da Solução (`MinhaApi.sln`):**
  * `src/MinhaApi.Dominio`: Entidades, Enums, Validadores e Serviços com as regras de negócio.
  * `src/MinhaApi.Api`: Camada de exposição Web API.
  * `tests/MinhaApi.Testes`: Suíte de testes unitários automatizados cobrindo fluxos de sucesso, cálculos e tratamento de exceções.

---

## Fluxo de Versionamento (Gitflow)

O repositório segue a estratégia **Gitflow** para isolamento e rastreabilidade das entregas:

* `main`: Versão estável de produção.
* `develop`: Branch de integração contínua das funcionalidades.
* `feature/*`: Desenvolvimento isolado de cada User Story do backlog (`feature/cadastro-cliente`, `feature/registro-venda`).
* `release/*`: Preparação, revisão e documentação da versão (`release/release-v.1.0`).
* `hotfix/*`: Correções emergenciais aplicadas diretamente sobre a `main` e replicadas na `develop`.

---

## Como Executar o Projeto e os Testes

### Pré-requisitos
* [.NET SDK 8.0](https://dotnet.microsoft.com/download) (ou superior compatível)

### Passos no Terminal

```bash
# 1. Clonar o repositório
git clone [https://github.com/ATV-GestaoDeQualidade/api.git](https://github.com/ATV-GestaoDeQualidade/api.git)
cd api

# 2. Restaurar dependências e compilar a solução
dotnet build

# 3. Executar todos os testes unitários
dotnet test --verbosity normal