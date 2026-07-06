       IDENTIFICATION DIVISION.
       PROGRAM-ID. PROJFINAL.
       AUTHOR. CAIO FELIPE.

       ENVIRONMENT DIVISION.
       CONFIGURATION SECTION.

       INPUT-OUTPUT SECTION.
       FILE-CONTROL.
           SELECT ARQ-ENT ASSIGN TO "runtime/entrada.txt"
               ORGANIZATION IS LINE SEQUENTIAL.

           SELECT ARQ-SAI ASSIGN TO "runtime/saida.txt"
               ORGANIZATION IS LINE SEQUENTIAL.

           SELECT ARQ-HLP-ENT ASSIGN TO "runtime/helper-entrada.txt"
               ORGANIZATION IS LINE SEQUENTIAL.

           SELECT ARQ-HLP-SAI ASSIGN TO "runtime/helper-saida.txt"
               ORGANIZATION IS LINE SEQUENTIAL.

       DATA DIVISION.
       FILE SECTION.

       FD  ARQ-ENT.
       01  REG-ENT              PIC X(117).

       FD  ARQ-SAI.
       01  REG-SAI              PIC X(188).

       FD  ARQ-HLP-ENT.
       01  REG-HLP-ENT          PIC X(117).

       FD  ARQ-HLP-SAI.
       01  REG-HLP-SAI          PIC X(176).

       WORKING-STORAGE SECTION.

       01  WKR-VALIDO           PIC X VALUE 'S'.
       01  WKR-I                PIC 9(03) VALUE 0.
       01  WKR-QTD-ARROBA       PIC 9(02) VALUE 0.
       01  WKR-TEM-PONTO        PIC X VALUE 'N'.
       01  WKR-CHAR             PIC X.
       01  WKR-COMANDO          PIC X(200).

           COPY CLIENTE.
           COPY SAIDA.
           COPY HELPER.

       01  WKR-OPERACAO         PIC X(10).
       01  WKR-CODIGO           PIC X(06).
       01  WKR-NOME             PIC X(30).
       01  WKR-EMAIL            PIC X(60).
       01  WKR-TELEFONE         PIC X(11).
       01  WKR-RETORNO          PIC X(04).
       01  WKR-MENSAGEM         PIC X(60).

       PROCEDURE DIVISION.

       0000-PRINCIPAL.
           PERFORM 0100-INICIAR.
           PERFORM 0200-ABRIR-ARQUIVOS.
           PERFORM 0300-LER-ENTRADA.

           IF WKR-VALIDO = 'S'
              PERFORM 0400-VALIDAR
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 1000-GRAVAR-ENTRADA-HELPER
              PERFORM 1100-CHAMAR-HELPER
              PERFORM 1200-LER-RETORNO-HELPER
           END-IF.

           PERFORM 0900-GRAVAR-SAIDA.
           PERFORM 0950-FECHAR-ARQUIVOS.
           STOP RUN.

       0100-INICIAR.
           MOVE SPACES TO WKR-ENTRADA.
           MOVE SPACES TO WKR-HELPER.
           MOVE SPACES TO WKR-RET-HELPER.
           MOVE SPACES TO WKR-OPERACAO.
           MOVE SPACES TO WKR-CODIGO.
           MOVE SPACES TO WKR-NOME.
           MOVE SPACES TO WKR-EMAIL.
           MOVE SPACES TO WKR-TELEFONE.
           MOVE '0000' TO WKR-RETORNO.
           MOVE SPACES TO WKR-MENSAGEM.
           MOVE SPACES TO WKR-COMANDO.
           MOVE 'S' TO WKR-VALIDO.

           MOVE SPACES TO SAI-RETORNO.
           MOVE SPACES TO SAI-MENSAGEM.
           MOVE SPACES TO SAI-OPERACAO.
           MOVE SPACES TO SAI-CODIGO.
           MOVE SPACES TO SAI-NOME.
           MOVE SPACES TO SAI-EMAIL.
           MOVE SPACES TO SAI-TELEFONE.

           MOVE '|' TO SAI-SEP-1.
           MOVE '|' TO SAI-SEP-2.
           MOVE '|' TO SAI-SEP-3.
           MOVE '|' TO SAI-SEP-4.
           MOVE '|' TO SAI-SEP-5.
           MOVE '|' TO SAI-SEP-6.

       0200-ABRIR-ARQUIVOS.
           OPEN INPUT ARQ-ENT.
           OPEN OUTPUT ARQ-SAI.

       0300-LER-ENTRADA.
           READ ARQ-ENT INTO WKR-ENTRADA
              AT END
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'ENTRADA VAZIA.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
           END-READ.

       0400-VALIDAR.
           MOVE ENT-OPERACAO TO WKR-OPERACAO.
           MOVE ENT-CODIGO TO WKR-CODIGO.
           MOVE ENT-NOME TO WKR-NOME.
           MOVE ENT-EMAIL TO WKR-EMAIL.
           MOVE ENT-TELEFONE TO WKR-TELEFONE.

           IF WKR-OPERACAO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'OPERACAO OBRIG.' TO WKR-MENSAGEM
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
                 PERFORM 0500-VALIDAR-CONSULTA
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO = 'CADASTRAR'
                 PERFORM 0600-VALIDAR-CADASTRO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-OPERACAO = 'ATUALIZAR'
                 PERFORM 0700-VALIDAR-ATUALIZACAO
              END-IF
           END-IF.

       0500-VALIDAR-CONSULTA.
           IF WKR-CODIGO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'CODIGO OBRIGATORIO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0800-VALIDAR-CODIGO
           END-IF.

       0600-VALIDAR-CADASTRO.
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
              PERFORM 0820-VALIDAR-EMAIL
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-TELEFONE NOT = SPACES
                 PERFORM 0850-VALIDAR-TELEFONE
              END-IF
           END-IF.

       0700-VALIDAR-ATUALIZACAO.
           IF WKR-CODIGO = SPACES
              MOVE '0422' TO WKR-RETORNO
              MOVE 'CODIGO OBRIGATORIO.' TO WKR-MENSAGEM
              MOVE 'N' TO WKR-VALIDO
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0800-VALIDAR-CODIGO
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-EMAIL = SPACES
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'EMAIL OBRIGATORIO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
           END-IF.

           IF WKR-VALIDO = 'S'
              PERFORM 0820-VALIDAR-EMAIL
           END-IF.

           IF WKR-VALIDO = 'S'
              IF WKR-TELEFONE NOT = SPACES
                 PERFORM 0850-VALIDAR-TELEFONE
              END-IF
           END-IF.

       0800-VALIDAR-CODIGO.
           MOVE 1 TO WKR-I.
           PERFORM UNTIL WKR-I > 6 OR WKR-VALIDO = 'N'
              MOVE WKR-CODIGO(WKR-I:1) TO WKR-CHAR
              IF WKR-CHAR < '0' OR WKR-CHAR > '9'
                 MOVE '0422' TO WKR-RETORNO
                 MOVE 'CODIGO NAO NUMERICO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
              ADD 1 TO WKR-I
           END-PERFORM.

       0820-VALIDAR-EMAIL.
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
                 MOVE 'TELEFONE INVALIDO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
              END-IF
              ADD 1 TO WKR-I
           END-PERFORM.

       0900-GRAVAR-SAIDA.
           MOVE WKR-RETORNO TO SAI-RETORNO.
           MOVE WKR-MENSAGEM TO SAI-MENSAGEM.
           MOVE WKR-OPERACAO TO SAI-OPERACAO.
           MOVE WKR-CODIGO TO SAI-CODIGO.
           MOVE WKR-NOME TO SAI-NOME.
           MOVE WKR-EMAIL TO SAI-EMAIL.
           MOVE WKR-TELEFONE TO SAI-TELEFONE.

           WRITE REG-SAI FROM WKR-SAIDA.

       0950-FECHAR-ARQUIVOS.
           CLOSE ARQ-ENT.
           CLOSE ARQ-SAI.

       1000-GRAVAR-ENTRADA-HELPER.
           MOVE WKR-OPERACAO TO HLP-OPERACAO.
           MOVE WKR-CODIGO TO HLP-CODIGO.
           MOVE WKR-NOME TO HLP-NOME.
           MOVE WKR-EMAIL TO HLP-EMAIL.
           MOVE WKR-TELEFONE TO HLP-TELEFONE.

           OPEN OUTPUT ARQ-HLP-ENT.
           WRITE REG-HLP-ENT FROM WKR-HELPER.
           CLOSE ARQ-HLP-ENT.

       1100-CHAMAR-HELPER.
           MOVE SPACES TO WKR-COMANDO.

           STRING
              'dotnet ' DELIMITED BY SIZE
              'src/Helper/bin/Debug/net10.0/' DELIMITED BY SIZE
              'Cooperativa.Helper.dll ' DELIMITED BY SIZE
              'ARQUIVO' DELIMITED BY SIZE
              INTO WKR-COMANDO
           END-STRING.

           CALL "SYSTEM" USING WKR-COMANDO.

       1200-LER-RETORNO-HELPER.
           OPEN INPUT ARQ-HLP-SAI.

           READ ARQ-HLP-SAI INTO WKR-RET-HELPER
              AT END
                 MOVE '0500' TO WKR-RETORNO
                 MOVE 'HELPER SEM RETORNO.' TO WKR-MENSAGEM
                 MOVE 'N' TO WKR-VALIDO
           END-READ.

           CLOSE ARQ-HLP-SAI.

           IF WKR-VALIDO = 'S'
              MOVE RHP-RETORNO TO WKR-RETORNO
              MOVE RHP-MENSAGEM TO WKR-MENSAGEM
              MOVE RHP-CODIGO TO WKR-CODIGO
              MOVE RHP-NOME TO WKR-NOME
              MOVE RHP-EMAIL TO WKR-EMAIL
              MOVE RHP-TELEFONE TO WKR-TELEFONE
           END-IF.
           