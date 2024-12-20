using FirstLineTamping.Configuration;
using FirstLineTamping.Contract.Authorities;

namespace ConfigurationTests
{
    public class StrongNameConfigTestSample : ConfigBase
    {
        /// <summary>
        /// 构造函数，
        /// </summary>
        public StrongNameConfigTestSample() : base(new Config())
        {
        }

        /// <summary>
        /// 定义作用域(包)
        /// 在多个配置文件合并的时候，用来区分不同的作用域
        /// </summary>
        public override string PathName => "Employee";

        /// <summary>
        /// 年龄
        /// </summary>
        [ConfigAuthorize(Authority.Anonymous | Authority.Operator,
            Authority.Engineer | Authority.Remote)]
        public int Age
        {
            get
            {
                return GetPropertyValue(0);
            }
            set
            {
                SetPropertyValue(value);
            }
        }
    }
}
