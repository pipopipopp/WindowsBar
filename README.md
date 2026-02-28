# WDZ – Process Monitor System

Sistema de monitoramento de processos em tempo real composto por:

- 🖥 **Windows Service (C#)** – Monitora processos no computador
- 🌐 **API Web (JavaScript)** – Recebe e armazena os dados
- 📊 **Web Interface (HTML/CSS/JS)** – Exibe os processos abertos em tempo real

---

## 🚀 Como Funciona

1. O serviço Windows roda em segundo plano  
2. Detecta abertura e fechamento de processos específicos  
3. Envia os dados para a API  
4. A webpage consome a API e exibe:
   - Processos abertos no momento
   - Quantidade de vezes que cada processo foi aberto

---

## 🛠 Tecnologias Utilizadas

- C# (.NET)
- JavaScript (Node.js)
- HTML
- CSS
- Git & GitHub
- Render (deploy da API)

---

## 📂 Estrutura

WDZ/
├── Serviço/
├── API/
└── Webpage/


---

## 🎯 Objetivo

Projeto pessoal focado em:

- Monitoramento de aplicações
- Comunicação entre serviço local e API
- Exibição de dados em tempo real
- Estrutura profissional de backend + frontend