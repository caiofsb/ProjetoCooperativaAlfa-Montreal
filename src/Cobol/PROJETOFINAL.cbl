       IDENTIFICATION DIVISION.
       PROGRAM-ID. PROJFINAL.
       AUTHOR. CAIO FELIPE.

       ENVIRONMENT DIVISION.
       CONFIGURATION SECTION.

       DATA DIVISION.
       WORKING-STORAGE SECTION.

       01  WKR-FIM              PIC X VALUE 'N'.
       01  WKR-VALIDO           PIC X VALUE 'S'.
       01  WKR-I                PIC 9(03) VALUE 0.
       01  WKR-QTD-ARROBA       PIC 9(02) VALUE 0.
       01  WKR-TEM-PONTO        PIC X VALUE 'N'.
       01  WKR-CHAR             PIC X.

       01  WKR-OPERACAO         PIC X(10).
       01  WKR-CODIGO           PIC X(06).
       01  WKR-NOME             PIC X(30).
       01  WKR-EMAIL            PIC X(60).
       01  WKR-TELEFONE         PIC X(11).
       01  WKR-RETORNO          PIC X(04).
       01  WKR-MENSAGEM         PIC X(60).

       01  WKR-ENTRADA.
           05  ENT-OPERACAO     PIC X(10).
           05  ENT-CODIGO       PIC X(06).
           05  ENT-NOME         PIC X(30).
           05  ENT-EMAIL        PIC X(60).
           05  ENT-TELEFONE     PIC X(11).

       01  WKR-SAIDA.
           05  SAI-RETORNO      PIC X(04).
           05  SAI-MENSAGEM     PIC X(60).

       PROCEDURE DIVISION.

       0000-PRINCIPAL.
           PERFORM 0100-INICIAR.
           PERFORM 0200-CARREGAR-EXEMPLO.
           PERFORM 0300-VALIDAR.
           PERFORM 0900-MOSTRAR-RETORNO.
           STOP RUN.

       0100-INICIAR.
           MOVE SPACES TO WKR-OPERACAO.
           MOVE SPACES TO WKR-CODIGO.
           MOVE SPACES TO WKR-NOME.
           MOVE SPACES TO WKR-EMAIL.
           MOVE SPACES TO WKR-TELEFONE.
           MOVE '0000' TO WKR-RETORNO.
           MOVE SPACES TO WKR-MENSAGEM.
           MOVE SPACES TO WKR-ENTRADA.
           MOVE SPACES TO WKR-SAIDA.
           MOVE 'S' TO WKR-VALIDO.

       0200-CARREGAR-EXEMPLO.
           MOVE 'CADASTRAR' TO ENT-OPERACAO.
           MOVE SPACES TO ENT-CODIGO.
           MOVE 'MARIA OLIVEIRA' TO ENT-NOME.
           MOVE 'maria.oliveira@cooperativa.com' TO ENT-EMAIL.
           MOVE SPACES TO ENT-TELEFONE.

       0300-VALIDAR.
           MOVE ENT-OPERACAO TO WKR-OPERACAO.
           MOVE ENT-CODIGO TO WKR-CODIGO.
           MOVE ENT-NOME TO WKR-NOME.
           MOVE ENT-EMAIL TO WKR-EMAIL.
           MOVE ENT-TELEFONE TO WKR-TELEFONE.

           IF WKR-OPERACAO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'OPERACAO OBRIGATORIA.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO NOT = 'CONSULTAR'
                 IF WKR-OPERACAO NOT = 'CADASTRAR'
                    IF WKR-OPERACAO NOT = 'ATUALIZAR'
                       MOVE '0422' TO WKR-RETORNO
                       MOVE 'OPERACAO INVALIDA.' TO WKR-MENSAGEM
                       MOVE 'N' TO WKR-VALIDO
                    END-IF
                 END-IF
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO = 'CONSULTAR'
                 PERFORM 0400-VALIDAR-CONSULTA
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO = 'CADASTRAR'
                 PERFORM 0500-VALIDAR-CADASTRO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO = 'ATUALIZAR'
                 PERFORM 0600-VALIDAR-ATUALIZACAO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              MOVE '0000' TO WKR-RETORNO
              MOVE 'VALIDACAO REALIZADA.' TO WKR-MENSAGEM
           END-IF.

       0400-VALIDAR-CONSULTA.
           IF WKR-CODIGO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'CODIGO OBRIGATORIO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0700-VALIDAR-CODIGO
           END-IF.

       0500-VALIDAR-CADASTRO.
           IF WKR-NOME = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'NOME OBRIGATORIO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-EMAIL = SPACES
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'EMAIL OBRIGATORIO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0800-VALIDAR-EMAIL
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-TELEFONE NOT = SPACES
                 PERFORM 0850-VALIDAR-TELEFONE
              END-IF
           END-IF.

       0600-VALIDAR-ATUALIZACAO.
           IF WKR-CODIGO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'CODIGO OBRIGATORIO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0700-VALIDAR-CODIGO
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-EMAIL = SPACES
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'EMAIL OBRIGATORIO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0800-VALIDAR-EMAIL
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-TELEFONE NOT = SPACES
                 PERFORM 0850-VALIDAR-TELEFONE
              END-IF
           END-IF.

       0700-VALIDAR-CODIGO.
           MOVE 1 TO WKR-I.
           PERFORM UNTIL WKR-I > 6 OR WKR-VALIDO = 'N'
              MOVE WKR-CODIGO(WKR-I:1) TO WKR-CHAR
              IF WKR-CHAR < '0' OR WKR-CHAR > '9'
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'CODIGO DEVE SER NUMERICO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
              ADD 1 TO WKR-I
           END-PERFORM.

       0800-VALIDAR-EMAIL.
           MOVE ZERO TO WKR-QTD-ARROBA.
           MOVE 'N' TO WKR-TEM-PONTO.
           MOVE 1 TO WKR-I.

           PERFORM UNTIL WKR-I > 60
              MOVE WKR-EMAIL(WKR-I:1) TO WKR-CHAR
              IF WKR-CHAR = '@'
                 ADD 1 TO WKR-QTD-ARROBA
              END-IF
              IF WKR-CHAR = '.'
                 MOVE 'S' TO WKR-TEM-PONTO
              END-IF
              ADD 1 TO WKR-I
           END-PERFORM.

           IF WKR-QTD-ARROBA NOT = 1
              MOVE '0422' TO WKR-RETORNO
              MOVE 'EMAIL INVALIDO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-TEM-PONTO NOT = 'S'
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'EMAIL INVALIDO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
           END-IF.

       0850-VALIDAR-TELEFONE.
           MOVE 1 TO WKR-I.
           PERFORM UNTIL WKR-I > 11 OR WKR-VALIDO = 'N'
              MOVE WKR-TELEFONE(WKR-I:1) TO WKR-CHAR
              IF WKR-CHAR < '0' OR WKR-CHAR > '9'
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'TELEFONE DEVE TER 11 DIGITOS.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
              ADD 1 TO WKR-I
           END-PERFORM.

       0900-MOSTRAR-RETORNO.
           MOVE WKR-RETORNO TO SAI-RETORNO.
           MOVE WKR-MENSAGEM TO SAI-MENSAGEM.

           DISPLAY 'RETORNO: ' SAI-RETORNO.
           DISPLAY 'MENSAGEM: ' SAI-MENSAGEM.