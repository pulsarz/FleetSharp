﻿using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Tests.TestVectors
{
    public static class MockedBoxes
    {
        public static List<Box<long>> RegularBoxes = new List<Box<long>>
        {
            new Box<long>
                {
                    boxId = "e56847ed19b3dc6b72828fcfb992fdf7310828cf291221269b7ffc72fd66706e",
                    value = 67500000000,
                    ergoTree = "100204a00b08cd021dde34603426402615658f1d970cfa7c7bd92ac81a8b16eeebff264d59ce4604ea02d192a39a8cc7a70173007301",
                    assets = new List<TokenAmount<long>>(),
                    creationHeight = 284761,
                    additionalRegisters = new NonMandatoryRegisters(),
                    transactionId = "9148408c04c2e38a6402a7950d6157730fa7d49e9ab3b9cadec481d7769918e9",
                    index = 1
                },
                new Box<long>
                {
                    boxId = "a2c9821f5c2df9c320f17136f043b33f7716713ab74c84d687885f9dd39d2c8a",
                    value = 1000000,
                    index = 0,
                    transactionId = "f82fa15166d787c275a6a5ab29983f6386571c63e50c73c1af7cba184f85ef23",
                    creationHeight = 805063,
                    ergoTree = "1012040204000404040004020406040c0408040a050004000402040204000400040404000400d812d601b2a4730000d602e4c6a7050ed603b2db6308a7730100d6048c720302d605db6903db6503fed606e4c6a70411d6079d997205b27206730200b27206730300d608b27206730400d609b27206730500d60a9972097204d60b95917205b272067306009d9c7209b27206730700b272067308007309d60c959272077208997209720a999a9d9c7207997209720b7208720b720ad60d937204720cd60e95720db2a5730a00b2a5730b00d60fdb6308720ed610b2720f730c00d6118c720301d612b2db630872127311009683060193c17212c1a793c27212c2a7938c7213017211938c721302997204720c93e4c67212050e720293e4c6721204117206",
                    assets = new List<TokenAmount<long>>
                    {
                        new TokenAmount<long>
                        {
                            tokenId = "007fd64d1ee54d78dd269c8930a38286caa28d3f29d27cadcb796418ab15c283",
                            amount = 226642336
                        }
                    },
                    additionalRegisters = new NonMandatoryRegisters
                    {
                        R4 = "110780f0b252a4048088bdfa9e60808c8d9e0200c80180f8efcc9f60",
                        R5 = "0e20aae3684e4e7c78c8d8e62f866345bd5c70543ac73f4a06142292f0693c9bdd60"
                    }
                },
                new Box<long>
                {
                    boxId = "467b6867c6726cc5484be3cbddbf55c30c0a71594a20c1ac28d35b5049632444",
                    transactionId = "b50333f95add30b71810d86579ec1b17add7c3ab186470cbb70ffe81d6260f44",
                    index = 1,
                    ergoTree = "0008cd038d39af8c37583609ff51c6a577efe60684119da2fbd0d75f9c72372886a58a63",
                    creationHeight = 804158,
                    value = 389063261,
                    assets = new List<TokenAmount<long>>
                    {
                        new TokenAmount<long>
                        {
                            tokenId = "5a34d53ca483924b9a6aa0c771f11888881b516a8d1a9cdc535d063fe26d065e",
                            amount = 33
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "bf2afb01fde7e373e22f24032434a7b883913bd87a23b62ee8b43eba53c9f6c2",
                            amount = 1
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "1fd6e032e8476c4aa54c18c1a308dce83940e8f4a28f576440513ed7326ad489",
                            amount = 896549
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "c4494f3bd96821f21d7a83b2baa640dfbe16f15853eb63a553bc840739b12f62",
                            amount = 1
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "a3b3fa62124ef52209a46121e3f93ca98d7fc24198009e90fde8205ef9d3fc33",
                            amount = 1
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "003bd19d0187117f130b62e1bcab0939929ff5c7709f843c5c4dd158949285d0",
                            amount = 1
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "bf59773def7e08375a553be4cbd862de85f66e6dd3dccb8f87f53158f9255bf5",
                            amount = 1234567890123456789L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "ee105e8290b090a773b7c56756507d45a76743d73bce54e8a915e95d9eb97360",
                            amount = 316227766L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "30974274078845f263b4f21787e33cc99e9ec19a17ad85a5bc6da2cca91c5a2e",
                            amount = 506432873054L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "007fd64d1ee54d78dd269c8930a38286caa28d3f29d27cadcb796418ab15c283",
                            amount = 27380L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "1c51c3a53abfe87e6db9a03c649e8360f255ffc4bd34303d30fc7db23ae551db",
                            amount = 540L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "fbbaac7337d051c10fc3da0ccb864f4d32d40027551e1c3ea3ce361f39b91e40",
                            amount = 1985L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "74251ce2cb4eb2024a1a155e19ad1d1f58ff8b9e6eb034a3bb1fd58802757d23",
                            amount = 200000000000L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "0cd8c9f416e5b1ca9f986a7f10a84191dfb85941619e49e53c0dc30ebf83324b",
                            amount = 3809L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "d601123e8838b95cdaebe24e594276b2a89cd38e98add98405bb5327520ecf6c",
                            amount = 15923500L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "d71693c49a84fbbecd4908c94813b46514b18b67a99952dc1e6e4791556de413",
                            amount = 1883L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "03faf2cb329f2e90d6d23b58d91bbb6c046aa143261cc21f52fbe2824bfcbf04",
                            amount = 50L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "ef802b475c06189fdbf844153cdc1d449a5ba87cce13d11bb47b5a539f27f12b",
                            amount = 1585999996837L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "bf337a2ce726259ad31e043c5b3d432e31b403fc6686691171e0e0a319b9ae7a",
                            amount = 1L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "36aba4b4a97b65be491cf9f5ca57b5408b0da8d0194f30ec8330d1e8946161c1",
                            amount = 3L
                        },
                        new TokenAmount<long>
                        {
                            tokenId = "02f31739e2e4937bb9afb552943753d1e3e9cdd1a5e5661949cb0cef93f907ea",
                            amount = 216926L
                        },
                    },
                    additionalRegisters = new NonMandatoryRegisters()
                },
                new Box<long>
                {
                    boxId = "30cb07d93f16f5b052e9f56c1b5dfb83db9ccaeb467dde064933afc23beb6f5f",
                    transactionId = "b50333f95add30b71810d86579ec1b17add7c3ab186470cbb70ffe81d6260f44",
                    index = 0,
                    ergoTree = "0008cd029a879c50408a569fa1a7661935759cf61fe770e4953359a73df17b91659723bd",
                    creationHeight = 804158,
                    value = 1000000000,
                    assets = new List<TokenAmount<long>>(),
                    additionalRegisters = new NonMandatoryRegisters()
                },
                new Box<long>
                {
                    boxId = "3e67b4be7012956aa369538b46d751a4ad0136138760553d5400a10153046e52",
                    transactionId = "22525acc8b9438ded1e0fef41bb38ac57b8be23c650c82dd8ba545ccdc0b97c2",
                    index = 0,
                    ergoTree = "0008cd03a621f820dbed198b42a2dca799a571911f2dabbd2e4d441c9aad558da63f084d",
                    creationHeight = 804138,
                    value = 1000000,
                    assets = new List<TokenAmount<long>>
                    {
                        new TokenAmount<long>
                        {
                            tokenId = "007fd64d1ee54d78dd269c8930a38286caa28d3f29d27cadcb796418ab15c283",
                            amount = 10000
                        }
                    },
                    additionalRegisters = new NonMandatoryRegisters()
                },
                new Box<long>
                {
                    boxId = "2555e34138d276905fe0bc19240bbeca10f388a71f7b4d2f65a7d0bfd23c846d",
                    transactionId = "b63017e55f17611a158caf0fd41c96c4453ff0cf761b99c319f3ac675cdfa8f2",
                    index = 0,
                    ergoTree = "0008cd03a621f820dbed198b42a2dca799a571911f2dabbd2e4d441c9aad558da63f084d",
                    creationHeight = 804138,
                    value = 1000000000,
                    assets = new List<TokenAmount<long>>
                    {
                        new TokenAmount<long>
                        {
                            tokenId = "0cd8c9f416e5b1ca9f986a7f10a84191dfb85941619e49e53c0dc30ebf83324b",
                            amount = 10
                        }
                    },
                    additionalRegisters = new NonMandatoryRegisters()
                }
        };

        public static List<ErgoUnsignedInput> RegularInputBoxes = RegularBoxes
            .Select(x => new ErgoUnsignedInput(new InputBox { boxId = x.boxId, ergoTree = x.ergoTree, additionalRegisters = x.additionalRegisters, assets = x.assets, creationHeight = x.creationHeight, index = x.index, transactionId = x.transactionId, value = x.value }))
            .ToList();
    }
}
