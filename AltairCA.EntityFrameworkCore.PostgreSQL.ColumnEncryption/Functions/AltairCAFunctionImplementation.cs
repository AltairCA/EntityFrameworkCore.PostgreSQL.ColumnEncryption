using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions
{
    internal class AltairCaFunctionImplementation:IMethodCallTranslator
    {
        public static string Password { get; set; }
        private readonly ISqlExpressionFactory _expressionFactory;

        private static readonly MethodInfo _encryptMethod
            = typeof(NpgsqlFunctionExtensions).GetMethod(
                nameof(NpgsqlFunctionExtensions.NpgsqlEncrypt),
                new[] { typeof(DbFunctions), typeof(string) });
        private static readonly MethodInfo _decryptMethod
            = typeof(NpgsqlFunctionExtensions).GetMethod(
                nameof(NpgsqlFunctionExtensions.NpgsqlDecrypt),
                new[] { typeof(DbFunctions), typeof(string) });

        private static readonly MethodInfo _encryptStringMethod
            = typeof(NpgsqlFunctionExtensions).GetMethod(
                nameof(NpgsqlFunctionExtensions.NpgEncrypt),
                new[] { typeof(string) });
        private static readonly MethodInfo _decryptStringMethod
           = typeof(NpgsqlFunctionExtensions).GetMethod(
               nameof(NpgsqlFunctionExtensions.NpgDecrypt),
               new[] { typeof(string) });

        private static readonly MethodInfo _nonTranslatableEncryptMethod
            = typeof(NpgsqlFunctionExtensions).GetMethod(
                nameof(NpgsqlFunctionExtensions.NpgEncrypt),
                new[] { typeof(string), typeof(string) });
        private static readonly MethodInfo __nonTranslatableDecryptMethod
           = typeof(NpgsqlFunctionExtensions).GetMethod(
               nameof(NpgsqlFunctionExtensions.NpgDecrypt),
               new[] { typeof(string), typeof(string) });
        public AltairCaFunctionImplementation(ISqlExpressionFactory expressionFactory)
        {
            _expressionFactory = expressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments,IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
           
            
            var password = _expressionFactory.Constant(Password);
            var algoConf = _expressionFactory.Constant("aes-cbc/pad:pkcs");
            
            if (method == _nonTranslatableEncryptMethod || method == __nonTranslatableDecryptMethod)
            {
                throw new Exception("Dont use this this will not translate to SQL. Use Decrypt() or Encrypt()");
            }
            if (method == _encryptStringMethod)
            {
                var value = arguments[0];
                
                var aesToHexExpression = AESEncryptionBuild(password, value,algoConf);

                return aesToHexExpression;
            }
            if (method == _encryptMethod)
            {
                var value = arguments[1];
                
                var aesToHexExpression = AESEncryptionBuild(password, value,algoConf);

                return aesToHexExpression;
            }

            if (method == _decryptMethod)
            {
                var value = arguments[1];
                
                var aesDecryptExpression = AesDecryptExpression(password, value,algoConf);
                return _expressionFactory.Convert(aesDecryptExpression, typeof(string));
            }
            if (method == _decryptStringMethod)
            {
                var value = arguments[0];

                var aesDecryptExpression = AesDecryptExpression(password, value,algoConf);
                return _expressionFactory.Convert(aesDecryptExpression, typeof(string));
            }
            return null;
        }

        private SqlFunctionExpression AesDecryptExpression( SqlExpression password, SqlExpression value, SqlExpression algoConf)
        {
            var hexExpression = _expressionFactory.Constant("base64");
            var decodeExpression = _expressionFactory.Function("decode", new List<SqlExpression>()
            {
                value,
                hexExpression
            }, typeof(byte[]));
            var decryptExpression = _expressionFactory.Function( "decrypt", new List<SqlExpression>()
            {
                decodeExpression,
                password,
                algoConf
            },typeof(byte[]));
            var decryptConvert = _expressionFactory.Convert(decryptExpression, typeof(byte[]));
            var convertFromExpression = _expressionFactory.Function("convert_from", new List<SqlExpression>()
            {
                decryptConvert,
                _expressionFactory.Constant("UTF-8")
            },typeof(string));
            return convertFromExpression;
        }

        protected virtual SqlFunctionExpression AESEncryptionBuild(SqlExpression password,
            SqlExpression value, SqlExpression algoConf)
        {
            var hexExpression = _expressionFactory.Constant("base64");
            var convertToBytea = _expressionFactory.Convert(value, typeof(byte[]));
            var aesEncryptionExpression = _expressionFactory.Function( "encrypt",
                new List<SqlExpression>()
                {
                    convertToBytea,
                    password,
                    algoConf
                }, typeof(byte[]));
            var aesToHexExpression = _expressionFactory.Function("encode", new List<SqlExpression>()
            {
                aesEncryptionExpression,
                hexExpression
            }, typeof(string));
            return aesToHexExpression;
        }
    }
}
